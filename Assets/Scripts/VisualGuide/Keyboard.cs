using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard : MonoBehaviour 
{
    string currentText;

    public void ClickKey (string character)
    {
        currentText += character;
        UIManager.Instance.leaderboard.UpdateCurrentPlayerName( currentText );
    }

    public void Backspace ()
    {
        if (currentText.Length > 0)
        {
            currentText = currentText.Substring(0, currentText.Length - 1);
            UIManager.Instance.leaderboard.UpdateCurrentPlayerName( currentText );
        }
    }

    public void Dismiss ()
    {
        currentText = "";
        gameObject.SetActive( false );
    }
}
