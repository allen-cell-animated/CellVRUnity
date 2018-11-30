using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapsiblePanel : MonoBehaviour 
{
    public GameObject hotspot;
    public Animator panel;

    List<Collider> collidingControllers = new List<Collider>();

    void OnTriggerEnter (Collider other)
    {
        if (other.tag == "GameController")
        {
            if (collidingControllers.Count < 1)
            {
                SetPanel( true );
            }
            collidingControllers.Add( other );
        }
    }

    void OnTriggerExit (Collider other)
    {
        if (collidingControllers.Contains( other ))
        {
            collidingControllers.Remove( other );
            if (collidingControllers.Count < 1)
            {
                SetPanel( false );
            }
        }
    }

    void SetPanel (bool open)
    {
        panel.SetTrigger( open ? "Open" : "Close" );
        hotspot.SetActive( !open );
    }
}
