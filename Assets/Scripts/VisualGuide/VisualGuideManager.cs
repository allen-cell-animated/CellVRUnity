using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualGuideManager : MonoBehaviour 
{
    public VisualGuideData data;
    public bool inIsolationMode;

    Vector3 startScale;
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

    public void LabelStructure (CellStructure _structure, Vector3 _position)
    {
        structureLabel.gameObject.SetActive( true );
        structureLabel.SetLabel( _structure.structureName, _position );
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
        infoPanel.gameObject.SetActive( true );
        Debug.Log(_structure == null);
        infoPanel.SetContent( _structure.structureName, _structure.infoImage, _structure.description );
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

    public void StartScaling ()
    {
        startScale = transform.localScale;
    }

    public void UpdateScale (float _scale)
    {
        transform.localScale = ClampScale( _scale * startScale );
    }

    Vector3 ClampScale (Vector3 scale)
    {
        if (scale.magnitude > maxScale.magnitude)
        {
            return maxScale;
        }
        else if (scale.magnitude < minScale.magnitude)
        {
            return minScale;
        }
        else
        {
            return scale;
        }
    }
}
