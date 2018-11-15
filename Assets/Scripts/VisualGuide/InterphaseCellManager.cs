using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterphaseCellManager : MonoBehaviour 
{
    bool inIsolationMode;
    CellStructure highlightedStructure;
    CellStructure selectedStructure;
    Vector3 lobbyPosition;
    Quaternion lobbyRotation;

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

    CellStructure[] _structures;
    CellStructure[] structures
    {
        get
        {
            if (_structures == null)
            {
                _structures = GetComponentsInChildren<CellStructure>();
            }
            return _structures;
        }
    }

    Transformer _transformer;
    public Transformer transformer
    {
        get
        {
            if (_transformer == null)
            {
                _transformer = GetComponent<Transformer>();
            }
            return _transformer;
        }
    }

    Mover _mover;
    public Mover mover
    {
        get
        {
            if (_mover == null)
            {
                _mover = gameObject.AddComponent<Mover>();
            }
            return _mover;
        }
    }

    Rotator _rotator;
    public Rotator rotator
    {
        get
        {
            if (_rotator == null)
            {
                _rotator = gameObject.AddComponent<Rotator>();
            }
            return _rotator;
        }
    }

    void Start ()
    {
        HideLabel();
        lobbyPosition = transform.position;
        lobbyRotation = transform.rotation;
    }

    void Update ()
    {
        if (inIsolationMode && ControllerInput.Instance.leftTriggerDown)
        {
            ExitIsolationMode();
        }
    }

    public void TransitionToPlayMode (MitosisGameManager currentGameManager)
    {
        HideLabel();
        mover.MoveToOverDuration( currentGameManager.targetDistanceFromCenter * Vector3.forward + currentGameManager.targetHeight * Vector3.up, 2f );
        rotator.RotateToOverDuration( Quaternion.Euler( new Vector3( -18f, -60f, 27f) ), 2f );
    }

    public void TransitionToLobbyMode ()
    {
        ExitIsolationMode();
        mover.MoveToOverDuration( lobbyPosition, 2f );
        rotator.RotateToOverDuration( lobbyRotation, 2f );
        transformer.enabled = false;
    }

    public void LabelStructure (CellStructure _structure)
    {
        highlightedStructure = _structure;
        structureLabel.gameObject.SetActive( true );
        structureLabel.SetLabel( _structure.structureName, _structure.nameWidth );
        transformer.enabled = true;
    }

    public void HideLabel (CellStructure _structure = null)
    {
        if (_structure == null || _structure == highlightedStructure)
        {
            structureLabel.gameObject.SetActive( false );
        }
    }

    public void SelectStructure (CellStructure _structure)
    {
        selectedStructure = _structure;
        IsolateSelectedStructure();
        VisualGuideManager.Instance.StartGame( _structure.structureName );
    }

    void IsolateSelectedStructure ()
    {
        if (selectedStructure != null && !inIsolationMode)
        {
            inIsolationMode = true;
            foreach (CellStructure structure in structures)
            {
                if (structure != selectedStructure)
                {
                    structure.gameObject.SetActive( false );
                }
                else
                {
                    structure.theCollider.enabled = false;
                }
            }
        }
    }

    public void ExitIsolationMode ()
    {
        if (inIsolationMode)
        {
            foreach (CellStructure structure in structures)
            {
                structure.gameObject.SetActive( true );
                structure.theCollider.enabled = true;
            }
            inIsolationMode = false;
        }
    }
}
