using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownCanvas : MonoBehaviour 
{
    public Animator numbers;

    public void StartCountdown ()
    {
        numbers.SetTrigger( "play" );
    }

    public void FinishCountdown ()
    {
        gameObject.SetActive( false );
    }
}
