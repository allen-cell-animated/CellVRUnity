using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public Color highlightColor;

    Color defaultColor;

    Material _material;
    Material material
    {
        get
        {
            if (_material == null)
            {
                _material = GetComponent<MeshRenderer>().material;
            }
            return _material;
        }
    }

    void Start ()
    {
        defaultColor = material.GetColor("_Color");
    }

    public void SetHighlight (bool _highlight)
    {
        material.SetColor("_Color", _highlight ? highlightColor : defaultColor);
    }
}
