using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderWith3DHandle : Slider
{
    SliderHandle3D _handle3D;
    SliderHandle3D handle3D
    {
        get
        {
            if (_handle3D == null)
            {
                _handle3D = GetComponentInChildren<SliderHandle3D>();
            }
            return _handle3D;
        }
    }

    public override void OnDrag (PointerEventData _data)
    {
        Debug.Log("drag");
        base.OnDrag( _data );
        handle3D.SetHighlight( true );
    }

    public override void OnPointerUp (PointerEventData _data)
    {
        Debug.Log("pointer up");
        base.OnDeselect(_data);
        handle3D.SetHighlight(false);
    }

    public override void OnPointerExit (PointerEventData _data)
    {
        //Debug.Log("pointer exit");
        base.OnDeselect(_data);
        //handle3D.SetHighlight(false);
    }
}
