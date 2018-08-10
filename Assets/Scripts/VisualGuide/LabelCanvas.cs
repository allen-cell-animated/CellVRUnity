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

        SetPanelSize();
        SetPosition( _cursorPosition );
    }

    void SetPanelSize ()
    {
        //TODO
    }

    void SetPosition (Vector3 _cursorPosition)
    {
        //TODO
    }
}
