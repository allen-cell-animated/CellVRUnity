using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour 
{
    public GameObject backButton;

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

    public void ToggleBackButton (bool on)
    {
        backButton.SetActive( on );
    }

    public void ExitPlayMode ()
    {
        VisualGuideManager.Instance.CleanupGame();
        VisualGuideManager.Instance.ReturnToLobby();
    }
}
