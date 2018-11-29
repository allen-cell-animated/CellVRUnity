using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard : MonoBehaviour 
{
    public Transform head;
    string currentText;

    void Start ()
    {
        transform.position = head.TransformPoint( new Vector3( 0, -0.5f, 1f ) );
        transform.rotation = head.rotation * Quaternion.Euler( new Vector3( 30f, 0, 0 ) );
    }

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
