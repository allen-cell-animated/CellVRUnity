using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CellStructure : VRTK_InteractableObject 
{
    [Header("Cell Structure Settings")]

    public bool hovering;
    public string structureName;
    public float nameWidth = 80f;
    public Color color;
    public Color illumColor;

    InterphaseCellManager _interphaseCell;
    InterphaseCellManager interphaseCell
    {
        get
        {
            if (_interphaseCell == null)
            {
                _interphaseCell = GetComponentInParent<InterphaseCellManager>();
            }
            return _interphaseCell;
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

    protected override void Awake ()
    {
        base.Awake();
        SetColor( false );
    }

    protected override void OnEnable ()
    {
        base.OnEnable();
        if (laserPointer != null)
        {
            laserPointer.DestinationMarkerEnter += OnHoverEnter;
            laserPointer.DestinationMarkerExit += OnHoverExit;
        }
    }

    protected override void OnDisable ()
    {
        base.OnDisable();
        if (laserPointer != null)
        {
            laserPointer.DestinationMarkerEnter -= OnHoverEnter;
            laserPointer.DestinationMarkerExit -= OnHoverExit;
        }
    }

    void OnHoverEnter (object sender, DestinationMarkerEventArgs e)
    {
        if (colliders.Find( c => c == e.raycastHit.collider ) != null)
        {
            hovering = true;
            interphaseCell.LabelStructure( this );
        }
    }

    void OnHoverExit (object sender, DestinationMarkerEventArgs e)
    {
        if (colliders.Find( c => c == e.raycastHit.collider ) != null)
        {
            interphaseCell.HideLabel( this );
            hovering = false;
        }
    }

    protected override void Update ()
    {
        base.Update();
        if (hovering && ControllerInput.Instance.rightTriggerDown)
        {
            interphaseCell.SelectStructure( this );
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
