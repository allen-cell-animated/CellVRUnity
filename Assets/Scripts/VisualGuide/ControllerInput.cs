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
    public bool rightGripDown;
    public bool leftGripDown;
    public bool leftTouchpadHover;
    public bool pointerHover;
    public bool touchingRight;
    public bool touchingLeft;

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

    VRTK_InteractTouch _toucherLeft;
    VRTK_InteractTouch toucherLeft
    {
        get
        {
            if (_toucherLeft == null && pointerLeft != null)
            {
                _toucherLeft = pointerLeft.GetComponent<VRTK_InteractTouch>();
            }
            return _toucherLeft;
        }
    }

    VRTK_InteractTouch _toucherRight;
    VRTK_InteractTouch toucherRight
    {
        get
        {
            if (_toucherRight == null && pointerRight != null)
            {
                _toucherRight = pointerRight.GetComponent<VRTK_InteractTouch>();
            }
            return _toucherRight;
        }
    }

    void OnEnable ()
    {
        if (pointerLeft != null)
        {
            pointerLeft.TriggerPressed += OnLeftControllerTriggerDown;
            pointerLeft.TriggerReleased += OnLeftControllerTriggerUp;
            pointerLeft.GripPressed += OnLeftControllerGripDown;
            pointerLeft.GripReleased += OnLeftControllerGripUp;
            pointerLeft.TouchpadTouchStart += OnLeftControllerTouchpadHoverEnter;
            pointerLeft.TouchpadTouchEnd += OnLeftControllerTouchpadHoverExit;
        }
        if (pointerRight != null)
        {
            pointerRight.TriggerPressed += OnRightControllerTriggerDown;
            pointerRight.TriggerReleased += OnRightControllerTriggerUp;
            pointerRight.GripPressed += OnRightControllerGripDown;
            pointerRight.GripReleased += OnRightControllerGripUp;
        }
        if (laserPointer != null)
        {
            laserPointer.DestinationMarkerEnter += OnPointerHoverEnter;
            laserPointer.DestinationMarkerExit += OnPointerHoverExit;
        }
        if (toucherLeft != null)
        {
            toucherLeft.ControllerTouchInteractableObject += OnLeftControllerTouch;
            toucherLeft.ControllerUntouchInteractableObject += OnLeftControllerUntouch;
        }
        if (toucherRight != null)
        {
            toucherRight.ControllerTouchInteractableObject += OnRightControllerTouch;
            toucherRight.ControllerUntouchInteractableObject += OnRightControllerUntouch;
        }
    }

    void OnDisable ()
    {
        if (pointerLeft != null)
        {
            pointerLeft.TriggerPressed -= OnLeftControllerTriggerDown;
            pointerLeft.TriggerReleased -= OnLeftControllerTriggerUp;
            pointerLeft.GripPressed -= OnLeftControllerGripDown;
            pointerLeft.GripReleased -= OnLeftControllerGripUp;
            pointerLeft.TouchpadTouchStart -= OnLeftControllerTouchpadHoverEnter;
            pointerLeft.TouchpadTouchEnd -= OnLeftControllerTouchpadHoverExit;
        }
        if (pointerRight != null)
        {
            pointerRight.TriggerPressed -= OnRightControllerTriggerDown;
            pointerRight.TriggerReleased -= OnRightControllerTriggerUp;
            pointerRight.GripPressed -= OnRightControllerGripDown;
            pointerRight.GripReleased -= OnRightControllerGripUp;
        }
        if (laserPointer != null)
        {
            laserPointer.DestinationMarkerEnter -= OnPointerHoverEnter;
            laserPointer.DestinationMarkerExit -= OnPointerHoverExit;
        }
        if (toucherLeft != null)
        {
            toucherLeft.ControllerTouchInteractableObject -= OnLeftControllerTouch;
            toucherLeft.ControllerUntouchInteractableObject -= OnLeftControllerUntouch;
        }
        if (toucherRight != null)
        {
            toucherRight.ControllerTouchInteractableObject -= OnRightControllerTouch;
            toucherRight.ControllerUntouchInteractableObject -= OnRightControllerUntouch;
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

    void OnRightControllerGripDown (object sender, ControllerInteractionEventArgs e)
    {
        rightGripDown = true;
    }

    void OnRightControllerGripUp (object sender, ControllerInteractionEventArgs e)
    {
        rightGripDown = false;
    }

    void OnLeftControllerGripDown (object sender, ControllerInteractionEventArgs e)
    {
        leftGripDown = true;
    }

    void OnLeftControllerGripUp (object sender, ControllerInteractionEventArgs e)
    {
        leftGripDown = false;
    }

    void OnLeftControllerTouchpadHoverEnter (object sender, ControllerInteractionEventArgs e)
    {
        leftTouchpadHover = true;
    }

    void OnLeftControllerTouchpadHoverExit (object sender, ControllerInteractionEventArgs e)
    {
        leftTouchpadHover = false;
    }

    void OnPointerHoverEnter (object sender, DestinationMarkerEventArgs e)
    {
        pointerHover = true;
    }

    void OnPointerHoverExit (object sender, DestinationMarkerEventArgs e)
    {
        pointerHover = false;
    }

    void OnRightControllerTouch (object sender, ObjectInteractEventArgs e)
    {
        touchingRight = true;
    }

    void OnRightControllerUntouch (object sender, ObjectInteractEventArgs e)
    {
        touchingRight = false;
    }

    void OnLeftControllerTouch (object sender, ObjectInteractEventArgs e)
    {
        touchingLeft = true;
    }

    void OnLeftControllerUntouch (object sender, ObjectInteractEventArgs e)
    {
        touchingLeft = false;
    }

    public void ToggleLaserRenderer (bool _active)
    {
        if (laserRenderer != null)
        {
            laserRenderer.enabled = false;
            laserRenderer.pointerRenderer.enabled = _active;
            laserRenderer.enabled = true;
        }
    }
}
