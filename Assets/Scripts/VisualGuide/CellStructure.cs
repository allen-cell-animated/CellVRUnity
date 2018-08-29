using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CellStructure : MonoBehaviour 
{
    public string structureName;
    public Color color;
    public bool showNucleus;
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

    List<Collider> _colliders;
    List<Collider> colliders
    {
        get
        {
            if (_colliders == null)
            {
                _colliders = new List<Collider>( GetComponentsInChildren<Collider>() );
            }
            return _colliders;
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
        if (colliders.Find( c => c == e.raycastHit.collider ) != null)
        {
            VisualGuideManager.Instance.OnHoverStructureEnter( this );
        }
    }

    void OnHoverExit (object sender, DestinationMarkerEventArgs e)
    {
        if (colliders.Find( c => c == e.raycastHit.collider ) != null)
        {
            VisualGuideManager.Instance.OnHoverStructureExit();
        }
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

    public void GrayOut (bool _gray)
    {
        foreach (Material material in materials)
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
}
