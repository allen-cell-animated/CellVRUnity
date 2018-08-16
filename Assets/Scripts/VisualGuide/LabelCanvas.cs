using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelCanvas : MonoBehaviour 
{
    public Text label;
    public RectTransform panel;

    public void SetLabel (string _label, Vector3 _cursorPosition)
    {
        label.text = _label;

        SetPanelSize( _label );
        SetPosition( _cursorPosition );
    }

    void SetPanelSize (string _label)
    {
        panel.sizeDelta = new Vector2( 6f * _label.Length + 6f, 20f );
    }

    void SetPosition (Vector3 _cursorPosition)
    {
        transform.position = _cursorPosition + 0.05f * Camera.main.transform.right;
        transform.LookAt( transform.position + (transform.position - Camera.main.transform.position) );
    }
}
