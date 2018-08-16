using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualGuideData : ScriptableObject 
{
    public List<StructureData> structureData;
}

[System.Serializable]
public class StructureData
{
    public string structureName;
    public Sprite infoImage;
    public string description;
    public float nameWidth;
}