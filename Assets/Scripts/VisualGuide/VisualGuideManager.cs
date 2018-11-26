using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VisualGuideGameMode
{
    Lobby,
    Play,
    Reward
}

public class VisualGuideManager : MonoBehaviour 
{
    public VisualGuideGameMode currentMode = VisualGuideGameMode.Lobby;
    public MitosisGameManager currentGameManager;

    string[] structureNames = { "Endoplasmic Reticulum", "Golgi Apparatus", "Microtubules", "Mitochondria"};
    Dictionary<string,bool> structuresSolved;
    Animator mitoticCellsAnimation;

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
        SetupPuzzles();
    }

    void SetupPuzzles ()
    {
        structuresSolved = new Dictionary<string, bool>();
        foreach (string structure in structureNames)
        {
            structuresSolved.Add( structure, false );
        }
    }

    public void StartGame (string structureName)
    {
        if (currentMode == VisualGuideGameMode.Lobby)
        {
            currentMode = VisualGuideGameMode.Play;
            CreateMitosisGameManager();
            currentGameManager.StartGame( structureName, 1.5f );
            interphaseCell.TransitionToPlayMode( currentGameManager );
            structuresSolved[structureName] = false;
            //ControllerInput.Instance.ToggleLaserRenderer( false );
            UIManager.Instance.ToggleBackButton( true );
        }
    }

    void CreateMitosisGameManager ()
    {
        GameObject prefab = Resources.Load( "MitosisGame" ) as GameObject;
        if (prefab == null)
        {
            Debug.LogWarning( "Couldn't load prefab for MitosisGame" );
            return;
        }
        currentGameManager = (Instantiate( prefab ) as GameObject).GetComponent<MitosisGameManager>();
    }

    public void StartSuccessAnimation ()
    {
        interphaseCell.MoveToCenter( 1f );
    }

    public void TriggerMitoticCellsAnimation ()
    {
        GameObject prefab = Resources.Load( currentGameManager.currentStructureName + "/MitoticCells" ) as GameObject;
        if (prefab == null)
        {
            Debug.LogWarning( "Couldn't load prefab for " + currentGameManager.currentStructureName + " MitoticCells!" );
        }
        mitoticCellsAnimation = (Instantiate( prefab, transform.position, transform.rotation, transform ) as GameObject).GetComponent<Animator>();
        mitoticCellsAnimation.SetTrigger( "Play" );
    }

    public void FinishSuccessAnimation ()
    {
        if (currentMode == VisualGuideGameMode.Play)
        {
            string structureName = currentGameManager.currentStructureName;
            structuresSolved[structureName] = true;

            CleanupGame();

            if (!allStructuresSolved)
            {
                ReturnToLobby( structureName );
            }
            else
            {
                SetupReward();
            }
        }
    }

    public void CleanupGame ()
    {
        if (currentMode == VisualGuideGameMode.Play)
        {
            Destroy( currentGameManager.gameObject );
            if (mitoticCellsAnimation != null)
            {
                Destroy( mitoticCellsAnimation.gameObject );
            }
        }
    }

    public void ReturnToLobby (string structureJustSolved = null)
    {
        currentMode = VisualGuideGameMode.Lobby;

        interphaseCell.gameObject.SetActive( true );
        interphaseCell.TransitionToLobbyMode( structureJustSolved );
        ControllerInput.Instance.ToggleLaserRenderer( true );
        UIManager.Instance.ToggleBackButton( false );
    }

    void SetupReward ()
    {
        currentMode = VisualGuideGameMode.Reward;
        CreateMitosisGameManager();
        StartCoroutine( currentGameManager.SpawnAllThrowables( structureNames ) );
    }
}
