using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelCanvas : MonoBehaviour 
{
    public Text label;
    public RectTransform panel;

    public void SetLabel (StructureData _data, Vector3 _cursorPosition)
    {
        label.text = _data.structureName;

        SetPanelSize( _data.nameWidth );
        SetPosition( _cursorPosition );
    }

    void SetPanelSize (float _width)
    {
        panel.sizeDelta = new Vector2( _width, 20f );
    }

    void SetPosition (Vector3 _cursorPosition)
    {
        transform.position = _cursorPosition + 0.05f * Camera.main.transform.right;
        transform.LookAt( transform.position + (transform.position - Camera.main.transform.position) );
    }
}
