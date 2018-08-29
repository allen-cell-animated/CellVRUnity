using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderHandle3D : MonoBehaviour 
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

    void OnMouseEnter ()
    {
        material.SetColor( "_Color", highlightColor );
    }

    void OnMouseExit ()
    {
        material.SetColor( "_Color", defaultColor );
    }
}
