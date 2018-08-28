using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CellStructure : MonoBehaviour 
{
    public string structureName;
    public Color color;
    [HideInInspector] public StructureData data;

    VRTK_DestinationMarker pointer
    {
        get
        {
            if (VisualGuideManager.Instance.pointerRight != null)
            {
                return VisualGuideManager.Instance.pointerRight.GetComponent<VRTK_DestinationMarker>();
            }
            return null;
        }
    }

    void OnEnable ()
    {
        if (pointer != null)
        {
            pointer.DestinationMarkerEnter += OnHoverEnter;
            pointer.DestinationMarkerExit += OnHoverExit;
        }
    }

    void OnDisable ()
    {
        if (pointer != null)
        {
            pointer.DestinationMarkerEnter -= OnHoverEnter;
            pointer.DestinationMarkerExit -= OnHoverExit;
        }
    }

    void OnHoverEnter (object sender, DestinationMarkerEventArgs e)
    {
        Debug.Log( "enter " + structureName );
        VisualGuideManager.Instance.OnHoverStructureEnter( this );
    }

    void OnHoverExit (object sender, DestinationMarkerEventArgs e)
    {
        Debug.Log("--------EXIT " + structureName);
        VisualGuideManager.Instance.OnHoverStructureExit();
    }

    void Start ()
    {
        GrayOut( false );
        SetData( VisualGuideManager.Instance.data );
    }

    public void SetData (VisualGuideData _data)
    {
        data = _data.structureData.Find( s => s.structureName == structureName );
        if (_data == null)
        {
            Debug.LogWarning( "Couldn't load structure data for " + structureName );
        }
    }

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

    public void GrayOut (bool _gray)
    {
        if (!_gray)
        {
            material.SetColor( "_Color", color );
        }
        else
        {
            material.SetColor( "_Color", Color.white );
        }
    }
}
