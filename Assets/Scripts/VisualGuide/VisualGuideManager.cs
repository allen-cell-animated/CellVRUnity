using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VisualGuideGameMode
{
    Lobby,
    Play
}

public class VisualGuideManager : MonoBehaviour 
{
    public VisualGuideGameMode currentMode = VisualGuideGameMode.Lobby;
    public MitosisGameManager currentGameManager;

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

    void Start ()
    {
        SetupPuzzles();
    }

    void SetupPuzzles ()
    {
        structuresSolved = new Dictionary<string, bool>();
        CellStructure[] structures = GetComponentsInChildren<CellStructure>();
        foreach (CellStructure structure in structures)
        {
            structuresSolved.Add( structure.structureName, false );
        }
    }

    public void StartGame (string structureName)
    {
        if (currentMode == VisualGuideGameMode.Lobby)
        {
            currentMode = VisualGuideGameMode.Play;
            CreateMitosisGameManager( structureName );
            interphaseCell.TransitionToPlayMode( currentGameManager );
            structuresSolved[structureName] = false;
            ControllerInput.Instance.ToggleLaserRenderer( false );
        }
    }

    void CreateMitosisGameManager (string structureName)
    {
        GameObject prefab = Resources.Load( "MitosisGame" ) as GameObject;
        if (prefab == null)
        {
            Debug.LogWarning( "Couldn't load prefab for MitosisGame" );
            return;
        }
        currentGameManager = (Instantiate( prefab ) as GameObject).GetComponent<MitosisGameManager>();
        currentGameManager.StartGame( structureName, 1.5f );
    }

    public void CompleteGame ()
    {
        interphaseCell.MoveToCenter( 1f );
    }

    public void FinishSuccessAnimation ()
    {
        GameObject prefab = Resources.Load( currentGameManager.currentStructureName + "/MitoticCells" ) as GameObject;
        if (prefab == null)
        {
            Debug.LogWarning( "Couldn't load prefab for " + currentGameManager.currentStructureName + " MitoticCells!" );
        }
        mitoticCellsAnimation = (Instantiate( prefab, transform.position, transform.rotation, transform ) as GameObject).GetComponent<Animator>();
        mitoticCellsAnimation.SetTrigger( "Play" );
    }

    public void CleanupGame ()
    {
        if (currentMode == VisualGuideGameMode.Play)
        {
            currentMode = VisualGuideGameMode.Lobby;

            string structureName = currentGameManager.currentStructureName;
            Destroy( currentGameManager.gameObject );
            Destroy( mitoticCellsAnimation.gameObject );

            interphaseCell.gameObject.SetActive( true );
            interphaseCell.TransitionToLobbyMode( structureName );

            structuresSolved[structureName] = true;
            ControllerInput.Instance.ToggleLaserRenderer( true );
        }
    }
}
