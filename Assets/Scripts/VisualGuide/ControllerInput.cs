using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ControllerInput : MonoBehaviour 
{
    public VRTK_ControllerEvents pointerLeft;
    public VRTK_ControllerEvents pointerRight;
    public bool rightTriggerDown;
    public bool leftTriggerDown;

    void OnEnable ()
    {
        if (pointerLeft != null && pointerRight != null)
        {
            pointerRight.TriggerPressed += OnRightControllerTriggerDown;
            pointerLeft.TriggerPressed += OnLeftControllerTriggerDown;
            pointerRight.TriggerReleased += OnRightControllerTriggerUp;
            pointerLeft.TriggerReleased += OnLeftControllerTriggerUp;
        }
    }

    void OnDisable ()
    {
        if (pointerLeft != null && pointerRight != null)
        {
            pointerRight.TriggerPressed -= OnRightControllerTriggerDown;
            pointerLeft.TriggerPressed -= OnLeftControllerTriggerDown;
            pointerRight.TriggerReleased -= OnRightControllerTriggerUp;
            pointerLeft.TriggerReleased -= OnLeftControllerTriggerUp;
        }
    }

    void OnRightControllerTriggerDown (object sender, ControllerInteractionEventArgs e)
    {
        rightTriggerDown = true;
    }

    void OnRightControllerTriggerUp (object sender, ControllerInteractionEventArgs e)
    {
        rightTriggerDown = false;
    }

    void OnLeftControllerTriggerDown (object sender, ControllerInteractionEventArgs e)
    {
        leftTriggerDown = true;
    }

    void OnLeftControllerTriggerUp (object sender, ControllerInteractionEventArgs e)
    {
        leftTriggerDown = false;
    }

    VRTK_Pointer _laser;
    VRTK_Pointer laser
    {
        get
        {
            if (_laser == null)
            {
                _laser = GameObject.FindObjectOfType<VRTK_Pointer>();
            }
            return _laser;
        }
    }

    public void ToggleLaser (bool _active)
    {
        laser.enabled = false;
        laser.pointerRenderer.enabled = _active;
        laser.enabled = true;
    }
}
