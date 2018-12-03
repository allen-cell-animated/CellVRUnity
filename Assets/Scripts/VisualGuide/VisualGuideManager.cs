using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VisualGuideGameMode
{
    Lobby,
    Play,
    Success
}

public class VisualGuideManager : MonoBehaviour 
{
    public VisualGuideData data;
    public VisualGuideGameMode currentMode = VisualGuideGameMode.Lobby;
    public MitosisGameManager currentGameManager;

    MitosisGameManager successGameManager;
    string[] structureNames = { "Endoplasmic Reticulum (ER)", "Golgi Apparatus", "Microtubules", "Mitochondria"};
    Dictionary<string,bool> structuresSolved;

    static VisualGuideManager _Instance;
    public static VisualGuideManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = GameObject.FindObjectOfType<VisualGuideManager>();
            }
            return _Instance;
        }
    }

    InterphaseCellManager _interphaseCell;
    public InterphaseCellManager interphaseCell
    {
        get
        {
            if (_interphaseCell == null)
            {
                _interphaseCell = GameObject.FindObjectOfType<InterphaseCellManager>();
            }
            return _interphaseCell;
        }
    }

    bool allStructuresSolved
    {
        get
        {
            foreach (KeyValuePair<string,bool> kvp in structuresSolved)
            {
                if (kvp.Value == false)
                {
                    return false;
                }
            }
            return true;
        }
    }

    void Start ()
    {
        ResetSolvedStructures();
    }

    public void ResetSolvedStructures ()
    {
        structuresSolved = new Dictionary<string, bool>();
        foreach (string structure in structureNames)
        {
            structuresSolved.Add( structure, false );
        }
        interphaseCell.SetColorsetForStructures( 0 );
    }

    public void StartGame (string structureName)
    {
        currentMode = VisualGuideGameMode.Play;

        structuresSolved[structureName] = false;
        UIManager.Instance.progressCanvas.SetTitle( structureName );
        UIManager.Instance.progressCanvas.SetSelected( structureName, false );
        UIManager.Instance.structureInfoCanvas.SetContent( data.structureData.Find( s => s.structureName == structureName ) );

        Cleanup();
        currentGameManager = CreateMitosisGameManager();
        currentGameManager.StartGame( structureName, 1.5f );

        interphaseCell.TransitionToPlayMode( currentGameManager );
        ControllerInput.Instance.ToggleLaserRenderer( false );
    }

    MitosisGameManager CreateMitosisGameManager ()
    {
        GameObject prefab = Resources.Load( "MitosisGame" ) as GameObject;
        if (prefab == null)
        {
            Debug.LogWarning( "Couldn't load prefab for MitosisGame" );
            return null;
        }
        return (Instantiate( prefab ) as GameObject).GetComponent<MitosisGameManager>();
    }

    public void EnterSuccessMode (float elapsedTime)
    {
        currentMode = VisualGuideGameMode.Success;

        structuresSolved[currentGameManager.currentStructureName] = true;
        UIManager.Instance.progressCanvas.SetSelected( currentGameManager.currentStructureName, true );
        interphaseCell.ColorActiveStructure();
        UIManager.Instance.DisplayScore( elapsedTime );
        ControllerInput.Instance.ToggleLaserRenderer( true );

        AnimateSuccess( interphaseCell.gameObject );
        currentGameManager.AnimateCellsForSuccess();
        CheckSucess();
    }

    public void AnimateSuccess (GameObject obj)
    {
        GameObject prefab = Resources.Load( "CellAnimator" ) as GameObject;
        if (prefab == null)
        {
            Debug.LogWarning( "Couldn't load prefab for CellAnimator" );
            return;
        }
        CellAnimator cellAnimator = (Instantiate( prefab ) as GameObject).GetComponent<CellAnimator>();

        cellAnimator.oldParent = obj.transform.parent;
        cellAnimator.transform.position = obj.transform.position;

        Animator animator = cellAnimator.GetComponentInChildren<Animator>();
        obj.transform.SetParent( animator.transform );
        animator.SetTrigger( "Success" );
    }

    void CheckSucess ()
    {
        if (allStructuresSolved)
        {
            successGameManager = CreateMitosisGameManager();
            StartCoroutine( successGameManager.SpawnAllThrowables( structureNames ) );
        }
    }

    public void ReturnToLobby ()
    {
        currentMode = VisualGuideGameMode.Lobby;

        Cleanup();

        interphaseCell.TransitionToLobbyMode();
        ControllerInput.Instance.ToggleLaserRenderer( true );
        UIManager.Instance.progressCanvas.SetTitle();
        UIManager.Instance.leaderboard.gameObject.SetActive( false );
        UIManager.Instance.keyboard.Dismiss();
    }

    void Cleanup ()
    {
        if (currentGameManager != null)
        {
            Destroy( currentGameManager.gameObject );
        }
        if (successGameManager != null)
        {
            Destroy( successGameManager.gameObject );
        }
    }
}
