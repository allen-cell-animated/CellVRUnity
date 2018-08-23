using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualGuideManager : MonoBehaviour 
{
    public bool canScale = true;
    public bool canRotate = true;
    public VisualGuideData data;
    public bool inIsolationMode;
    public bool scaling;
    public bool rotating;
    public bool hasTranslated;

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
        infoPanel.gameObject.SetActive( false );

        LoadStructureData();
    }

    void LoadStructureData ()
    {
        foreach (CellStructure structure in structures)
        {
            structure.SetData( data.structureData.Find( s => s.structureName == structure.structureName ) );
        }
    }

    public void LabelStructure (CellStructure _structure)
    {
        if (!scaling && !rotating)
        {
            structureLabel.gameObject.SetActive( true );
            structureLabel.SetLabel( _structure.data );
        }
    }

    public void HideLabel ()
    {
        structureLabel.gameObject.SetActive( false );
    }

    public void IsolateStructure (CellStructure _structure)
    {
        inIsolationMode = true;
        foreach (CellStructure structure in structures)
        {
            if (structure != _structure && !structure.alwaysShowInIsolationMode)
            {
                structure.gameObject.SetActive( false );
            }
        }
        ShowInfoPanel( _structure );
    }

    void ShowInfoPanel (CellStructure _structure)
    {
        infoPanel.SetContent( _structure.data );
        infoPanel.gameObject.SetActive( true );
    }

    public void ExitIsolationMode ()
    {
        foreach (CellStructure s in structures)
        {
            s.gameObject.SetActive( true );
        }
        infoPanel.gameObject.SetActive( false );
        inIsolationMode = false;
    }

    public void StartScaling (Vector3 _controller1Position, Vector3 _controller2Position)
    {
        if (canScale)
        {
            scaling = true;
            startScale = transform.localScale;
            startControllerDistance = Vector3.Distance( _controller1Position, _controller2Position );
        }
    }

    public void UpdateScale (Vector3 _controller1Position, Vector3 _controller2Position)
    {
        if (canScale)
        {
            float scale = Vector3.Distance( _controller1Position, _controller2Position ) / startControllerDistance;

            transform.localScale = ClampScale( scale * startScale );
        }
    }

    public void StopScaling ()
    {
        scaling = false;
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

    public void StartRotating (Vector3 _controller1Position, Vector3 _controller2Position)
    {
        if (canRotate)
        {
            rotating = true;
            startRotation = transform.localRotation;
            startControllerVector = _controller2Position - _controller1Position;
            startControllerVector.y = 0;
            startPositiveVector = Vector3.Cross( startControllerVector, Vector3.up );
        }
    }

    public void UpdateRotation (Vector3 _controller1Position, Vector3 _controller2Position)
    {
        if (canRotate)
        {
            Vector3 controllerVector = _controller2Position - _controller1Position;
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

    public void StopRotating ()
    {
        rotating = false;
    }
}
