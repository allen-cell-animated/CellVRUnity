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
            }
        }
    }

    public void ExitIsolationMode ()
    {
        if (inIsolationMode)
        {
            foreach (CellStructure s in structures)
            {
                s.gameObject.SetActive( true );
            }
            inIsolationMode = false;
        }
    }
}
