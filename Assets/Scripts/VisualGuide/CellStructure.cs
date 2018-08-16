using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellStructure : MonoBehaviour 
{
    public string structureName;
    [HideInInspector] public StructureData data;
    public GameObject outline;
    public bool alwaysShowInIsolationMode;

    public void SetData (StructureData _data)
    {
        if (_data != null)
        {
            data = _data;
        }
        else
        {
            Debug.LogWarning( "Couldn't load structure data for " + structureName );
        }
    }

    public void SetOutline (bool _show)
    {
        outline.SetActive( _show );
    }
}
