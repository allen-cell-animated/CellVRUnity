using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterphaseCellManager : MonoBehaviour 
{
    bool inIsolationMode;
    CellStructure highlightedStructure;
    CellStructure selectedStructure;
    Vector3 lobbyPosition;
    Quaternion lobbyRotation;
    float lobbyScale;

    LabelCanvas _structureLabel;
    LabelCanvas structureLabel
    {
        get
        {
            if (_structureLabel == null)
            {
                _structureLabel = GameObject.FindObjectOfType<LabelCanvas>();
            }
            return _structureLabel;
        }
    }

    List<CellStructure> _structures;
    List<CellStructure> structures
    {
        get
        {
            if (_structures == null)
            {
                _structures = new List<CellStructure>( GetComponentsInChildren<CellStructure>() );
            }
            return _structures;
        }
    }

    Transformer _transformer;
    Transformer transformer
    {
        get
        {
            if (_transformer == null)
            {
                _transformer = GetComponent<Transformer>();
            }
            return _transformer;
        }
    }

    Mover _mover;
    Mover mover
    {
        get
        {
            if (_mover == null)
            {
                _mover = gameObject.AddComponent<Mover>();
            }
            return _mover;
        }
    }

    Rotator _rotator;
    Rotator rotator
    {
        get
        {
            if (_rotator == null)
            {
                _rotator = gameObject.AddComponent<Rotator>();
            }
            return _rotator;
        }
    }

    Scaler _scaler;
    Scaler scaler
    {
        get
        {
            if (_scaler == null)
            {
                _scaler = gameObject.AddComponent<Scaler>();
            }
            return _scaler;
        }
    }

    Animator _animator;
    Animator animator
    {
        get
        {
            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
            }
            return _animator;
        }
    }

    bool canInteract 
    {
        get
        {
            return !transformer.transforming && VisualGuideManager.Instance.currentMode == VisualGuideGameMode.Lobby;
        }
    }

    void Start ()
    {
        RemoveHighlightAndLabel( null, true );
        lobbyPosition = transform.position;
        lobbyRotation = transform.rotation;
        lobbyScale = transform.localScale.x;
    }

    public void TransitionToPlayMode (MitosisGameManager currentGameManager)
    {
        float duration = 1f;
        transformer.enabled = false;
        mover.MoveToOverDuration( currentGameManager.targetDistanceFromCenter * Vector3.forward + currentGameManager.targetHeight * Vector3.up, duration );
        rotator.RotateToOverDuration( Quaternion.Euler( new Vector3( -18f, -60f, 27f) ), duration );
        scaler.ScaleOverDuration( lobbyScale, duration );
        StartCoroutine( currentGameManager.TurnOffInterphaseCellTarget( duration ) );
        RemoveHighlightAndLabel( null, true );
    }

    public void TransitionToLobbyMode ()
    {
        ExitIsolationMode();
        transformer.enabled = true;
        MoveToCenter( 1f );
    }

    public void MoveToCenter (float duration)
    {
        mover.MoveToOverDuration( lobbyPosition, duration );
        rotator.RotateToOverDuration( lobbyRotation, duration );
        scaler.ScaleOverDuration( lobbyScale, duration );
    }

    public void HighlightAndLabelStructure (CellStructure _structure)
    {
        if (canInteract)
        {
            Debug.Log("ADD " + (_structure == null ? "null" : _structure.structureName));
            highlightedStructure = _structure;
            structureLabel.gameObject.SetActive( true );
            structureLabel.SetLabel( _structure.structureName, _structure.nameWidth );
            ColorNonHighlightedStructures( 0 );
        }
    }

    public void RemoveHighlightAndLabel (CellStructure _structure, bool force = false)
    {
        Debug.Log("Remove " + (_structure == null ? "null" : _structure.structureName) + " " + force);
        if (structureLabel != null && (_structure == highlightedStructure || force))
        {
            highlightedStructure = null;
            structureLabel.gameObject.SetActive( false );
            ColorNonHighlightedStructures( 1 );
        }
    }

    void ColorNonHighlightedStructures (int colorset)
    {
        foreach (CellStructure structure in structures)
        {
            if (structure != highlightedStructure)
            {
                structure.colorer.SetColor( colorset );
            }
        }
    }

    public void SelectStructure (string _structureName)
    {
        CellStructure _structure = structures.Find( s => s.structureName == _structureName );
        if (_structure != null)
        {
            selectedStructure = _structure;
            IsolateSelectedStructure();
            VisualGuideManager.Instance.StartGame( _structureName );
        }
    }

    void IsolateSelectedStructure ()
    {
        if (selectedStructure != null && !inIsolationMode)
        {
            inIsolationMode = true;
            foreach (CellStructure structure in structures)
            {
                if (structure != selectedStructure && !structure.isNucleus)
                {
                    structure.gameObject.SetActive( false );
                }
                else
                {
                    structure.theCollider.enabled = false;
                }
            }
        }
    }

    void ExitIsolationMode ()
    {
        if (inIsolationMode)
        {
            foreach (CellStructure structure in structures)
            {
                structure.gameObject.SetActive( true );
                structure.theCollider.enabled = true;
            }
            inIsolationMode = false;
        }
    }
}
