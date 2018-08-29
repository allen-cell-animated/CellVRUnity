using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderHandle3D : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
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

    void Start ()
    {
        defaultColor = material.GetColor( "_Color" );
    }

    public void OnPointerEnter (PointerEventData _data)
    {
        Debug.Log( "ENTER" );
        material.SetColor( "_Color", highlightColor );
    }

    public void OnPointerDown (PointerEventData _data) { }

    public void OnPointerUp (PointerEventData _data)
    {
        Debug.Log( "exit" );
        material.SetColor( "_Color", defaultColor );
    }
}
