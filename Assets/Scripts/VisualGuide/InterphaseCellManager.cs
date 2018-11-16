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
        HideLabel();
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
        currentGameManager.TurnOffInterphaseCellTarget( duration );
        structures.Find( s => s.structureName == currentGameManager.currentStructureName ).SetColor( false );
        HideLabel();
    }

    public void MoveToCenter (float duration)
    {
        mover.MoveToOverDuration( lobbyPosition, duration, ArrivedAtCenter );
        rotator.RotateToOverDuration( lobbyRotation, duration );
        scaler.ScaleOverDuration( lobbyScale, duration );
    }

    public void ArrivedAtCenter ()
    {
        VisualGuideManager.Instance.currentGameManager.AnimateNextPhase();
    }

    public void TransitionToLobbyMode (string structureJustSolved)
    {
        ExitIsolationMode();
        transform.position = lobbyPosition;
        transform.rotation = lobbyRotation;
        transform.localScale = lobbyScale * Vector3.one;
        structures.Find( s => s.structureName == structureJustSolved ).SetColor( true );
        transformer.enabled = true;
        animator.SetTrigger( "Success" );
    }

    public void LabelStructure (CellStructure _structure)
    {
        if (canInteract)
        {
            highlightedStructure = _structure;
            structureLabel.gameObject.SetActive( true );
            structureLabel.SetLabel( _structure.structureName, _structure.nameWidth );
        }
    }

    public void HideLabel (CellStructure _structure = null)
    {
        if (structureLabel != null && (_structure == null || _structure == highlightedStructure))
        {
            structureLabel.gameObject.SetActive( false );
        }
    }

    public void SelectStructure (CellStructure _structure)
    {
        if (canInteract)
        {
            selectedStructure = _structure;
            IsolateSelectedStructure();
            VisualGuideManager.Instance.StartGame( _structure.structureName );
        }
    }

    void IsolateSelectedStructure ()
    {
        if (selectedStructure != null && !inIsolationMode)
        {
            inIsolationMode = true;
            foreach (CellStructure structure in structures)
            {
                if (structure != selectedStructure)
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

    public void ExitIsolationMode ()
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
