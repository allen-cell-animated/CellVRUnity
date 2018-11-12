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

    static ControllerInput _Instance;
    public static ControllerInput Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = GameObject.FindObjectOfType<ControllerInput>();
            }
            return _Instance;
        }
    }

    void OnEnable ()
    {
        if (pointerLeft != null)
        {
            pointerLeft.TriggerPressed += OnLeftControllerTriggerDown;
            pointerLeft.TriggerReleased += OnLeftControllerTriggerUp;
        }
        if (pointerRight != null)
        {
            pointerRight.TriggerPressed += OnRightControllerTriggerDown;
            pointerRight.TriggerReleased += OnRightControllerTriggerUp;
        }
    }

    void OnDisable ()
    {
        if (pointerLeft != null)
        {
            pointerLeft.TriggerPressed -= OnLeftControllerTriggerDown;
            pointerLeft.TriggerReleased -= OnLeftControllerTriggerUp;
        }
        if (pointerRight != null)
        {
            pointerRight.TriggerPressed -= OnRightControllerTriggerDown;
            pointerRight.TriggerReleased -= OnRightControllerTriggerUp;
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

    VRTK_Pointer _laserRenderer;
    VRTK_Pointer laserRenderer
    {
        get
        {
            if (_laserRenderer == null)
            {
                _laserRenderer = GameObject.FindObjectOfType<VRTK_Pointer>();
            }
            return _laserRenderer;
        }
    }

    public void ToggleLaserRenderer (bool _active)
    {
        _laserRenderer.enabled = false;
        _laserRenderer.pointerRenderer.enabled = _active;
        _laserRenderer.enabled = true;
    }

    VRTK_DestinationMarker _laserPointer;
    public VRTK_DestinationMarker laserPointer
    {
        get
        {
            if (_laserPointer == null && pointerRight != null)
            {
                _laserPointer = pointerRight.GetComponent<VRTK_DestinationMarker>();
            }
            return _laserPointer;
        }
    }
}
