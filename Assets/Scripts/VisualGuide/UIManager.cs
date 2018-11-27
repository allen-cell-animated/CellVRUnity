using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class UIManager : MonoBehaviour 
{
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
    }
}
