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
    string[] structureNames = { "Microtubules", "Mitochondria", "Endoplasmic Reticulum (ER)", "Golgi Apparatus" };
    int currentStuctureIndex;

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

    string nextStructureName 
    {
        get
        {
            return structureNames[currentStuctureIndex];
        }
    }

    public void SelectNextStructureAndPlay ()
    {
        interphaseCell.SelectStructure( structureNames[currentStuctureIndex] );
    }

    public void StartGame (string structureName)
    {
        Debug.Log( "-------start game with " + structureName );
        currentMode = VisualGuideGameMode.Play;

        UIManager.Instance.EnterPlayMode( data.structureData.Find( s => s.structureName == structureName ) );

        Cleanup();
        currentGameManager = CreateMitosisGameManager();
        currentGameManager.StartGame( structureName, 1.5f );

        interphaseCell.TransitionToPlayMode( currentGameManager );
        ControllerInput.Instance.ToggleLaserRenderer( false );
        ControllerInput.Instance.laserDisabledUnlessPointingAtUI = true;
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

        UIManager.Instance.EnterSuccessMode( currentGameManager.currentStructureName, elapsedTime );

        AnimateCellSuccess( interphaseCell.gameObject );
        currentGameManager.AnimateCellsForSuccess();
        successGameManager = CreateMitosisGameManager();
        StartCoroutine( successGameManager.SpawnAllThrowables( structureNames ) );

        currentStuctureIndex++;
        if (currentStuctureIndex >= structureNames.Length)
        {
            currentStuctureIndex = 0;
        }
    }

    public void AnimateCellSuccess (GameObject cell)
    {
        GameObject prefab = Resources.Load( "CellAnimator" ) as GameObject;
        if (prefab == null)
        {
            Debug.LogWarning( "Couldn't load prefab for CellAnimator" );
            return;
        }
        CellAnimator cellAnimator = (Instantiate( prefab ) as GameObject).GetComponent<CellAnimator>();

        cellAnimator.oldParent = cell.transform.parent;
        cellAnimator.transform.position = cell.transform.position;

        Animator animator = cellAnimator.GetComponentInChildren<Animator>();
        cell.transform.SetParent( animator.transform );
        animator.SetTrigger( "Success" );

        prefab = Resources.Load( "Confetti2" ) as GameObject;
        if (prefab == null)
        {
            Debug.LogWarning( "Couldn't load prefab for Confetti2" );
            return;
        }
        Instantiate( prefab, cell.transform.position, Quaternion.identity );
    }

    public void ReturnToLobby ()
    {
        currentMode = VisualGuideGameMode.Lobby;

        Cleanup();

        interphaseCell.TransitionToLobbyMode();
        ControllerInput.Instance.ToggleLaserRenderer( true );
        ControllerInput.Instance.laserDisabledUnlessPointingAtUI = false;
        UIManager.Instance.EnterLobbyMode( structureNames[currentStuctureIndex] );
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
