﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VisualGuideManager : MonoBehaviour 
{
    public VRTK_ControllerEvents pointerLeft;
    public VRTK_ControllerEvents pointerRight;
    public VisualGuideData data;
    public CellStructure selectedStructure;
    public bool canScale = true;
    public bool canRotate = true;

    bool rightTriggerDown;
    bool leftTriggerDown;
    bool translating;
    bool inIsolationMode;

    Vector3 startScale;
    float startControllerDistance;
    Quaternion startRotation;
    Vector3 startControllerVector;
    Vector3 startPositiveVector;
    Vector3 minScale = new Vector3( 0.2f, 0.2f, 0.2f );
    Vector3 maxScale = new Vector3( 10f, 10f, 10f );

    static VisualGuideManager _Instance;
    public static VisualGuideManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = GameObject.FindObjectOfType<VisualGuideManager>();
            }
            return _Instance;
        }
    }

    List<CellStructure> _structures;
    public List<CellStructure> structures
    {
        get
        {
            if (_structures == null)
            {
                _structures = new List<CellStructure>( GetComponentsInChildren<CellStructure>() );
            }
            return _structures;
        }
    }

    LabelCanvas _structureLabel;
    LabelCanvas structureLabel
    {
        get
        {
            if (_structureLabel == null)
            {
                _structureLabel = GameObject.FindObjectOfType<LabelCanvas>();
            }
            return _structureLabel;
        }
    }

    InfoCanvas _infoPanel;
    InfoCanvas infoPanel
    {
        get
        {
            if (_infoPanel == null)
            {
                _infoPanel = GameObject.FindObjectOfType<InfoCanvas>();
            }
            return _infoPanel;
        }
    }

    void Start ()
    {
        structureLabel.gameObject.SetActive( false );
    }

    // INPUT --------------------------------------------------------------------------------------------------

    void OnEnable ()
    {
        if (pointerLeft != null && pointerRight != null)
        {
            pointerRight.TriggerPressed += OnRightControllerTriggerDown;
            pointerLeft.TriggerPressed += OnLeftControllerTriggerDown;
            pointerRight.TriggerReleased += OnRightControllerTriggerUp;
            pointerLeft.TriggerReleased += OnLeftControllerTriggerUp;
        }
    }

    void OnDisable ()
    {
        if (pointerLeft != null && pointerRight != null)
        {
            pointerRight.TriggerPressed -= OnRightControllerTriggerDown;
            pointerLeft.TriggerPressed -= OnLeftControllerTriggerDown;
            pointerRight.TriggerReleased -= OnRightControllerTriggerUp;
            pointerLeft.TriggerReleased -= OnLeftControllerTriggerUp;
        }
    }

    void OnRightControllerTriggerDown (object sender, ControllerInteractionEventArgs e)
    {
        rightTriggerDown = true;
    }

    void OnRightControllerTriggerUp (object sender, ControllerInteractionEventArgs e)
    {
        rightTriggerDown = false;
        if (!translating)
        {
            IsolateSelectedStructure();
        }
    }

    void OnLeftControllerTriggerDown (object sender, ControllerInteractionEventArgs e)
    {
        leftTriggerDown = true;
    }

    void OnLeftControllerTriggerUp (object sender, ControllerInteractionEventArgs e)
    {
        leftTriggerDown = false;
    }

    void Update ()
    {
        if (rightTriggerDown && leftTriggerDown)
        {
            if (!translating)
            {
                translating = true;
                StartScaling();
                StartRotating();
            }
            else
            {
                UpdateScale();
                UpdateRotation();
            }
        }
        else
        {
            translating = false;
        }
    }

    // HIGHLIGHT & LABEL --------------------------------------------------------------------------------------------------

    public void OnHoverStructureEnter (CellStructure _selectedStructure)
    {
        Debug.Log( "select " + _selectedStructure );
        selectedStructure = _selectedStructure;
        foreach (CellStructure structure in structures)
        {
            if (structure != selectedStructure)
            {
                structure.GrayOut( true );
            }
        }
        selectedStructure.GrayOut( false );
        LabelSelectedStructure();
    }

    public void OnHoverStructureExit ()
    {
        Debug.Log( "--------- DESELECT " + selectedStructure );
        selectedStructure = null;
        foreach (CellStructure structure in structures)
        {
            structure.GrayOut( false );
        }
        HideLabel();
    }

    void LabelSelectedStructure ()
    {
        structureLabel.gameObject.SetActive( true );
        structureLabel.SetLabel( selectedStructure.data );
    }

    void HideLabel ()
    {
        structureLabel.gameObject.SetActive( false );
    }

    // ISOLATE STRUCTURE --------------------------------------------------------------------------------------------------

    void IsolateSelectedStructure ()
    {
        if (selectedStructure != null)
        {
            inIsolationMode = true;
            foreach (CellStructure structure in structures)
            {
                if (structure != selectedStructure)
                {
                    structure.gameObject.SetActive( false );
                }
            }
            ShowInfoPanelForSelectedStructure();
        }
    }

    void ShowInfoPanelForSelectedStructure ()
    {
        infoPanel.SetContent( selectedStructure.data );
        infoPanel.gameObject.SetActive( true );
    }

    void ExitIsolationMode ()
    {
        foreach (CellStructure s in structures)
        {
            s.gameObject.SetActive( true );
        }
        infoPanel.gameObject.SetActive( false );
        inIsolationMode = false;
    }

    // SCALING --------------------------------------------------------------------------------------------------

    void StartScaling ()
    {
        if (canScale)
        {
            startScale = transform.localScale;
            startControllerDistance = Vector3.Distance( pointerRight.transform.position, pointerLeft.transform.position );
        }
    }

    void UpdateScale ()
    {
        if (canScale)
        {
            float scale = Vector3.Distance( pointerRight.transform.position, pointerLeft.transform.position ) / startControllerDistance;

            transform.localScale = ClampScale( scale * startScale );
        }
    }

    Vector3 ClampScale (Vector3 _scale)
    {
        if (_scale.magnitude > maxScale.magnitude)
        {
            return maxScale;
        }
        else if (_scale.magnitude < minScale.magnitude)
        {
            return minScale;
        }
        else
        {
            return _scale;
        }
    }

    // ROTATING --------------------------------------------------------------------------------------------------

    void StartRotating ()
    {
        if (canRotate)
        {
            startRotation = transform.localRotation;
            startControllerVector = pointerRight.transform.position - pointerLeft.transform.position;
            startControllerVector.y = 0;
            startPositiveVector = Vector3.Cross( startControllerVector, Vector3.up );
        }
    }

    void UpdateRotation ()
    {
        if (canRotate)
        {
            Vector3 controllerVector = pointerRight.transform.position - pointerLeft.transform.position;
            controllerVector.y = 0;
            float direction = GetArcCosineDegrees( Vector3.Dot( startPositiveVector.normalized, controllerVector.normalized ) ) >= 90f ? 1f : -1f;
            float dAngle = direction * GetArcCosineDegrees( Vector3.Dot( startControllerVector.normalized, controllerVector.normalized ) );

            transform.localRotation = startRotation * Quaternion.AngleAxis( dAngle, Vector3.up );
        }
    }

    float GetArcCosineDegrees (float cosine)
    {
        if (cosine > 1f - float.Epsilon)
        {
            return 0;
        }
        if (cosine < -1f + float.Epsilon)
        {
            return 180f;
        }
        return Mathf.Acos( cosine ) * Mathf.Rad2Deg;
    }
}
