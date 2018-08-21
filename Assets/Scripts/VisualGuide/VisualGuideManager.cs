using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualGuideManager : MonoBehaviour 
{
    public VisualGuideData data;
    public bool inIsolationMode;
    public bool scaling;

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

    SteamVR_LaserPointer _laserPointer;
    SteamVR_LaserPointer laserPointer
    {
        get
        {
            if (_laserPointer == null)
            {
                _laserPointer = GameObject.FindObjectOfType<SteamVR_LaserPointer>();
            }
            return _laserPointer;
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
        if (!scaling)
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

    void ToggleLaser (bool on)
    {
        laserPointer.pointer.SetActive( on );
    }

    public void StartScaling ()
    {
        scaling = true;
        startScale = transform.localScale;
        HideLabel();
        ToggleLaser( false );
    }

    public void UpdateScale (float _scale)
    {
        transform.localScale = ClampScale( _scale * startScale );
    }

    public void StopScaling ()
    {
        scaling = false;
        ToggleLaser( true );
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
