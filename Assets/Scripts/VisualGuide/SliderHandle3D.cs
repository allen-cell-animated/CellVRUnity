using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderHandle3D : Slider
{
    public Color highlightColor;
    public GameObject handle3D;

    Color defaultColor;

    Material _material;
    Material material
    {
        get
        {
            if (_material == null)
            {
                _material = handle3D.GetComponent<MeshRenderer>().material;
            }
            return _material;
        }
    }

    protected override void Start ()
    {
        base.Start();
        defaultColor = material.GetColor( "_Color" );
    }

    public override void OnDrag (PointerEventData _data)
    {
        base.OnDrag( _data );
        material.SetColor( "_Color", highlightColor );
    }

    public override void OnSelect (BaseEventData _data) 
    { 
        base.OnSelect( _data );
        material.SetColor( "_Color", highlightColor );
    }

    public override void OnDeselect (BaseEventData _data)
    {
        base.OnDeselect( _data );
        material.SetColor( "_Color", defaultColor );
    }
}
