using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CellStructure : VRTK_InteractableObject 
{
    public string structureName;
    public Color color;
    public Color illumColor;
    public GameObject nucleusToDisplayInIsolation;
    [HideInInspector] public StructureData data;

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

    public VRTK_DestinationMarker laserPointer
    {
        get
        {
            return ControllerInput.Instance.laserPointer;
        }
    }

    //protected override void OnEnable ()
    //{
    //    base.OnEnable();
    //    if (laserPointer != null)
    //    {
    //        laserPointer.DestinationMarkerEnter += OnHoverEnter;
    //        laserPointer.DestinationMarkerExit += OnHoverExit;
    //    }
    //}

    //protected override void OnDisable ()
    //{
    //    base.OnDisable();
    //    if (laserPointer != null)
    //    {
    //        laserPointer.DestinationMarkerEnter -= OnHoverEnter;
    //        laserPointer.DestinationMarkerExit -= OnHoverExit;
    //    }
    //}

    //void OnHoverEnter (object sender, DestinationMarkerEventArgs e)
    //{
    //    if (colliders.Find( c => c == e.raycastHit.collider ) != null)
    //    {
    //        MitosisGameManager.Instance.OnHoverStructureEnter( this );
    //    }
    //}

    //void OnHoverExit (object sender, DestinationMarkerEventArgs e)
    //{
    //    if (colliders.Find( c => c == e.raycastHit.collider ) != null)
    //    {
    //        MitosisGameManager.Instance.OnHoverStructureExit();
    //    }
    //}

    void Start ()
    {
        SetColor( false );
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

    public void SetColor (bool _colored)
    {
        foreach (Material material in materials)
        {
            if (_colored)
            {
                material.SetColor( "_Color", color );
                material.SetColor( "_EmissionColor", illumColor );
            }
            else
            {
                material.SetColor( "_Color", Color.white );
                material.SetColor( "_EmissionColor", Color.black );
            }
        }
    }
}
