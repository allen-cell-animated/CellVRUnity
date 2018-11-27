using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorSet
{
    public Color color;
    public Color illumColor;
}

public class Colorer : MonoBehaviour 
{
    public ColorSet[] colors;

    Material[] _materials;
    Material[] materials
    {
        get
        {
            if (_materials == null)
            {
                MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
                _materials = new Material[renderers.Length];
                for (int i = 0; i < renderers.Length; i++)
                {
                    _materials[i] = renderers[i].material;
                }
            }
            return _materials;
        }
    }

    public void SetColor (int colorIndex)
    {
        if (colorIndex < colors.Length)
        {
            foreach (Material material in materials)
            {
                material.SetColor( "_Color", colors[colorIndex].color );
                material.SetColor( "_EmissionColor", colors[colorIndex].illumColor );
            }
        }
    }
}
