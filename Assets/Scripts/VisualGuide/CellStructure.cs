using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CellStructure : VRTK_InteractableObject 
{
    [Header("Cell Structure Settings")]

    public string structureName;
    public float nameWidth = 80f;
    public Color color;
    public Color illumColor;
    public GameObject nucleusToDisplayInIsolation;
    [HideInInspector] public StructureData data;

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

    void Start ()
    {
        SetColor( false );
    }

    public override void StartUsing (VRTK_InteractUse currentUsingObject = null)
    {
        base.StartUsing( currentUsingObject );
        interphaseCell.LabelStructure( this );
    }

    public override void StopUsing (VRTK_InteractUse previousUsingObject = null, bool resetUsingObjectState = true)
    {
        base.StopUsing( previousUsingObject, resetUsingObjectState );
        interphaseCell.HideLabel( this );
    }

    protected override void Update ()
    {
        base.Update();
        if (IsUsing() && ControllerInput.Instance.rightTriggerDown)
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
