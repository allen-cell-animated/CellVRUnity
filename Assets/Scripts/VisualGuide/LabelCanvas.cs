using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelCanvas : MonoBehaviour 
{
    public Text label;
    public RectTransform panel;
    public Transform cursor;

    void Update ()
    {
        transform.position = cursor.transform.position;
        transform.rotation = cursor.transform.rotation;
    } 

    public void SetLabel (StructureData _data)
    {
        label.text = _data.structureName;

        SetPanelSize( _data.nameWidth );
    }

    void SetPanelSize (float _width)
    {
        panel.sizeDelta = new Vector2( _width, 20f );
    }
}
