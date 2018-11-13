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

    MitosisGameManager currentGameManager;
    Vector3 interphaseCellLobbyPosition;
    Quaternion interphaseCellLobbyRotation;

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
    InterphaseCellManager interphaseCell
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
        interphaseCellLobbyPosition = interphaseCell.transform.position;
        interphaseCellLobbyRotation = interphaseCell.transform.rotation;
    }

    public void StartGame (string structureName)
    {
        if (currentMode == VisualGuideGameMode.Lobby)
        {
            currentMode = VisualGuideGameMode.Play;
            GameObject prefab = Resources.Load( "MitosisGame" ) as GameObject;
            if (prefab == null)
            {
                Debug.LogWarning( "Couldn't load prefab for MitosisGame" );
                return;
            }

            currentGameManager = (Instantiate( prefab ) as GameObject).GetComponent<MitosisGameManager>();
            currentGameManager.StartGame( structureName, 3f );

            interphaseCell.mover.MoveToOverDuration( currentGameManager.targetDistanceFromCenter * Vector3.forward + currentGameManager.targetHeight * Vector3.up, 2f );
            interphaseCell.rotator.RotateToOverDuration( Quaternion.Euler( new Vector3( -18f, -60f, 27f) ), 2f );
        }
    }

    public void CompleteGame ()
    {
        if (currentMode == VisualGuideGameMode.Play)
        {
            currentMode = VisualGuideGameMode.Lobby;
            Destroy( currentGameManager.gameObject );
            interphaseCell.ExitIsolationMode();
            interphaseCell.mover.MoveToOverDuration( interphaseCellLobbyPosition, 2f );
            interphaseCell.rotator.RotateToOverDuration( interphaseCellLobbyRotation, 2f );
        }
    }
}
