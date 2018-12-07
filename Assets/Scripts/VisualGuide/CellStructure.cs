using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CellStructure : MonoBehaviour 
{
    public bool isNucleus;
    public string structureName;
    public float nameWidth = 80f;

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

    Collider _collider;
    public Collider theCollider
    {
        get
        {
            if (_collider == null)
            {
                _collider = GetComponent<Collider>();
            }
            return _collider;
        }
    }

    Colorer _colorer;
    public Colorer colorer
    {
        get
        {
            if (_colorer == null)
            {
                _colorer = GetComponent<Colorer>();
            }
            return _colorer;
        }
    }

    void OnEnable ()
    {
        if (ControllerInput.Instance.laserPointer != null)
        {
            ControllerInput.Instance.laserPointer.DestinationMarkerEnter += OnHoverEnter;
            ControllerInput.Instance.laserPointer.DestinationMarkerExit += OnHoverExit;
        }
    }

    void OnDisable ()
    {
        if (ControllerInput.Instance != null && ControllerInput.Instance.laserPointer != null)
        {
            ControllerInput.Instance.laserPointer.DestinationMarkerEnter -= OnHoverEnter;
            ControllerInput.Instance.laserPointer.DestinationMarkerExit -= OnHoverExit;
        }
    }

    void OnHoverEnter (object sender, DestinationMarkerEventArgs e)
    {
        if (theCollider == e.raycastHit.collider)
        {
            interphaseCell.HighlightAndLabelStructure( this );
        }
    }

    void OnHoverExit (object sender, DestinationMarkerEventArgs e)
    {
        if (theCollider == e.raycastHit.collider)
        {
            interphaseCell.RemoveHighlightAndLabel( this );
        }
    }
}
