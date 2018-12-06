using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class UIManager : MonoBehaviour 
{
    public InfoCanvas dataInfoCanvas;
    public InfoCanvas structureInfoCanvas;
    public ProgressCanvas progressCanvas;
    public Leaderboard leaderboard;
    public Keyboard keyboard;
    public GameObject playbutton;
    public Text nextStructureLabel;

    static UIManager _Instance;
    public static UIManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = GameObject.FindObjectOfType<UIManager>();
            }
            return _Instance;
        }
    }

    void OnEnable ()
    {
        if (ControllerInput.Instance.pointerLeft != null)
        {
            ControllerInput.Instance.pointerLeft.TouchpadReleased += OnLeftControllerTouchpadUp;
        }
    }

    void OnDisable ()
    {
        if (ControllerInput.Instance.pointerLeft != null)
        {
            ControllerInput.Instance.pointerLeft.TouchpadReleased -= OnLeftControllerTouchpadUp;
        }
    }

    void OnLeftControllerTouchpadUp (object sender, ControllerInteractionEventArgs e)
    {
        if (VisualGuideManager.Instance.currentMode != VisualGuideGameMode.Lobby)
        {
            VisualGuideManager.Instance.ReturnToLobby();
        }
    }

    void Start ()
    {
        leaderboard.gameObject.SetActive( false );
        structureInfoCanvas.transform.parent.gameObject.SetActive( false );
        progressCanvas.time.gameObject.SetActive( false );
    }

    void Update ()
    {
        if (Input.GetKeyUp( KeyCode.X ))
        {
            VisualGuideManager.Instance.ResetSolvedStructures();
        }

        if (Input.GetKeyUp( KeyCode.C ))
        {
            //toggle color in integrated cell in lobby
        }

        if (Input.GetKeyUp( KeyCode.Q ))
        {
            //toggle cell placement in game
        }

        if (Input.GetKey( KeyCode.A ) && Input.GetKey( KeyCode.I ) && Input.GetKey( KeyCode.C ) && Input.GetKey( KeyCode.S ))
        {
            leaderboard.ClearAllRankings();
        }
    }

    public void UpdateTime (float startTime)
    {
        progressCanvas.time.text = FormatTime( Time.time - startTime );
    }

    public string FormatTime (float timeSeconds)
    {
        float timeMinutes = Mathf.Floor( timeSeconds / 60f );
        timeSeconds = Mathf.Round( timeSeconds - 60f * timeMinutes);

        string timeSecondsStr = timeSeconds.ToString();
        while (timeSecondsStr.Length < 2)
        {
            timeSecondsStr = "0" + timeSecondsStr;
        }

        string timeMinutesStr = timeMinutes.ToString();
        while (timeMinutesStr.Length < 2)
        {
            timeMinutesStr = "0" + timeMinutesStr;
        }

        return timeMinutesStr + ":" + timeSecondsStr;
    }

    public void DisplayScore (float elapsedTime)
    {
        leaderboard.gameObject.SetActive( true );
        leaderboard.RecordNewScore( elapsedTime );
        keyboard.gameObject.SetActive( true );
    }

    public void EnterPlayMode (StructureData structureData)
    {
        progressCanvas.AnimateTitle();
        progressCanvas.SetSelected( structureData.structureName, true );
        progressCanvas.time.gameObject.SetActive( true );
        progressCanvas.gameObject.SetActive( true );
        structureInfoCanvas.SetContent( structureData );
        dataInfoCanvas.transform.parent.gameObject.SetActive( false );
        structureInfoCanvas.transform.parent.gameObject.SetActive( true );
        playbutton.SetActive( false );
    }

    public void EnterSuccessMode (string structureName, float timeSeconds)
    {
        DisplayScore( timeSeconds );
    }

    public void EnterLobbyMode (string currentStructureName)
    {
        progressCanvas.gameObject.SetActive( false );
        dataInfoCanvas.transform.parent.gameObject.SetActive( true );
        structureInfoCanvas.transform.parent.gameObject.SetActive( false );
        playbutton.SetActive( true );
        nextStructureLabel.text = "Next: " + currentStructureName;
    }

    public void Play ()
    {
        VisualGuideManager.Instance.SelectNextStructureAndPlay();
    }
}
