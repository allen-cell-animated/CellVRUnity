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

    Animator _animator;
    Animator animator
    {
        get
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
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
    }

    public void TransitionToPlayMode (MitosisGameManager currentGameManager)
    {
        transformer.enabled = false;
        mover.MoveToOverDuration( currentGameManager.targetDistanceFromCenter * Vector3.forward + currentGameManager.targetHeight * Vector3.up, 2f );
        rotator.RotateToOverDuration( Quaternion.Euler( new Vector3( -18f, -60f, 27f) ), 2f );
        structures.Find( s => s.structureName == currentGameManager.currentStructureName ).SetColor( false );
        HideLabel();
    }

    public void TransitionToLobbyMode (string structureJustSolved)
    {
        ExitIsolationMode();
        mover.MoveToOverDuration( lobbyPosition, 2f );
        rotator.RotateToOverDuration( lobbyRotation, 2f );
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
        if (_structure == null || _structure == highlightedStructure)
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
