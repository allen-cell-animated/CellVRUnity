using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellStructure : MonoBehaviour 
{
    public string structureName;
    public GameObject outline;

    public void SetOutline (bool show)
    {
        outline.SetActive( show );
    }
}
