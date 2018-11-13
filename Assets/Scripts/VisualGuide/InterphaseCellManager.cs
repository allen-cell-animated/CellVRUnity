using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterphaseCellManager : MonoBehaviour 
{
    bool inIsolationMode;
    CellStructure highlightedStructure;
    CellStructure selectedStructure;

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
        structureLabel.gameObject.SetActive(false);
    }

    void Update ()
    {
        if (inIsolationMode && ControllerInput.Instance.leftTriggerDown)
        {
            ExitIsolationMode();
        }
    }

    public void LabelStructure (CellStructure _structure)
    {
        highlightedStructure = _structure;
        structureLabel.gameObject.SetActive( true );
        structureLabel.SetLabel( _structure.structureName, _structure.nameWidth );
    }

    public void HideLabel (CellStructure _structure)
    {
        if (_structure == highlightedStructure)
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
