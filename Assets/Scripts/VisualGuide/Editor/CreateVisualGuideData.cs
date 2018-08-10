using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateVisualGuideData 
{
    [MenuItem("Assets/Create/VisualGuideData")]
    public static void Create()
    {
        VisualGuideData asset = ScriptableObject.CreateInstance<VisualGuideData>();

        AssetDatabase.CreateAsset(asset, "Assets/Data/newVisualGuideData.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
