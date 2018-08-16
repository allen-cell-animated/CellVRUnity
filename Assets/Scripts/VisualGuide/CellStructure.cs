using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellStructure : MonoBehaviour 
{
    public string structureName;
    [HideInInspector] public Sprite infoImage;
    [HideInInspector] public string description;
    public GameObject outline;
    public bool alwaysShowInIsolationMode;

    public void SetData (StructureData _data)
    {
        if (_data != null)
        {
            infoImage = _data.infoImage;
            description = _data.description;
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
