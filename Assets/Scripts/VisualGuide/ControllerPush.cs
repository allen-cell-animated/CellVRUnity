using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ControllerPush : MonoBehaviour 
{
    bool pushEnabled = true;

    GameObject _pusher;
    GameObject pusher
    {
        get
        {
            if (_pusher == null)
            {
                _pusher = transform.Find( "Pusher" ).gameObject;
            }
            return _pusher;
        }
    }

    VRTK_ControllerEvents _pointer;
    VRTK_ControllerEvents pointer
    {
        get
        {
            if (_pointer == null)
            {
                _pointer = GetComponent<VRTK_ControllerEvents>();
            }
            return _pointer;
        }
    }

    void OnEnable ()
    {
        if (pointer != null)
        {
            pointer.TriggerPressed += OnControllerTriggerDown;
            pointer.TriggerReleased += OnControllerTriggerUp;
        }
    }

    void OnDisable ()
    {
        if (pointer != null)
        {
            pointer.TriggerPressed -= OnControllerTriggerDown;
            pointer.TriggerReleased -= OnControllerTriggerUp;
        }
    }

    void OnControllerTriggerDown (object sender, ControllerInteractionEventArgs e)
    {
        pusher.SetActive( false );
    }

    void OnControllerTriggerUp (object sender, ControllerInteractionEventArgs e)
    {
        pusher.SetActive( true );
    }
}
