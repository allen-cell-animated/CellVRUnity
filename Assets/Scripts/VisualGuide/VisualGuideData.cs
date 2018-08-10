using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualGuideData : ScriptableObject 
{
    public StructureData[] StructureData;
}

[System.Serializable]
public class StructureData
{
    public string structureName;
    public Sprite infoImage;
    public string description;
}