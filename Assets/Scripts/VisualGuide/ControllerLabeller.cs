using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ControllerLabeller : MonoBehaviour 
{
    public GameObject scaleButtonLabelRight;
    public GameObject grabButtonLabelRight;
    public GameObject selectButtonLabelRight;
    public GameObject gripRight;

    public GameObject scaleButtonLabelLeft;
    public GameObject grabButtonLabelLeft;
    public GameObject backButtonLabelLeft;
    public GameObject backButtonLabelLeftHover;
    public GameObject gripLeft;

    public float gripScaleMultiplier;

    float gripStartScale;

    void Awake ()
    {
        gripStartScale = gripLeft.transform.localScale.x;
    }

    void OnEnable ()
    {
        if (ControllerInput.Instance.pointerLeft != null)
        {
            ControllerInput.Instance.pointerLeft.GripPressed += OnLeftControllerGripDown;
            ControllerInput.Instance.pointerLeft.GripReleased += OnLeftControllerGripUp;
        }
        if (ControllerInput.Instance.pointerRight != null)
        {
            ControllerInput.Instance.pointerRight.GripPressed += OnRightControllerGripDown;
            ControllerInput.Instance.pointerRight.GripReleased += OnRightControllerGripUp;
        }
    }

    void OnDisable ()
    {
        if (ControllerInput.Instance.pointerLeft != null)
        {
            ControllerInput.Instance.pointerLeft.GripPressed -= OnLeftControllerGripDown;
            ControllerInput.Instance.pointerLeft.GripReleased -= OnLeftControllerGripUp;
        }
        if (ControllerInput.Instance.pointerRight != null)
        {
            ControllerInput.Instance.pointerRight.GripPressed -= OnRightControllerGripDown;
            ControllerInput.Instance.pointerRight.GripReleased -= OnRightControllerGripUp;
        }
    }

    void OnRightControllerGripDown (object sender, ControllerInteractionEventArgs e)
    {
        gripRight.transform.localScale = gripScaleMultiplier * gripStartScale * Vector3.one;
    }

    void OnRightControllerGripUp (object sender, ControllerInteractionEventArgs e)
    {
        gripRight.transform.localScale = gripStartScale * Vector3.one;
    }

    void OnLeftControllerGripDown (object sender, ControllerInteractionEventArgs e)
    {
        gripLeft.transform.localScale = gripScaleMultiplier * gripStartScale * Vector3.one;
    }

    void OnLeftControllerGripUp (object sender, ControllerInteractionEventArgs e)
    {
        gripLeft.transform.localScale = gripStartScale * Vector3.one;
    }

    void Update ()
    {
        UpdateButtonLabels();
    }

    void UpdateButtonLabels ()
    {
        if (VisualGuideManager.Instance.currentMode == VisualGuideGameMode.Lobby)
        {
            ShowObject( scaleButtonLabelRight, !ControllerInput.Instance.rightGripDown );
            ShowObject( scaleButtonLabelLeft, !ControllerInput.Instance.leftGripDown );

            //ShowObject( selectButtonLabelRight, !ControllerInput.Instance.rightTriggerDown && ControllerInput.Instance.pointerHover );

            ShowObject( gripRight, true );
            ShowObject( gripLeft, true );

            ShowObject( grabButtonLabelRight, false );
            ShowObject( grabButtonLabelLeft, false );
            ShowObject( backButtonLabelLeft, false );
        }
        else
        {
            ShowObject( grabButtonLabelRight, !ControllerInput.Instance.rightTriggerDown && ControllerInput.Instance.touchingRight );
            ShowObject( grabButtonLabelLeft, !ControllerInput.Instance.leftTriggerDown && ControllerInput.Instance.touchingLeft );

            ShowObject( backButtonLabelLeft, true );
            ShowObject( backButtonLabelLeftHover, ControllerInput.Instance.leftTouchpadHover );

            ShowObject( gripRight, false );
            ShowObject( gripLeft, false );
            ShowObject( scaleButtonLabelRight, false );
            ShowObject( scaleButtonLabelLeft, false );
            //ShowObject( selectButtonLabelRight, false );
        }
    }

    void ShowObject (GameObject obj, bool show)
    {
        if (obj != null)
        {
            if (show && !obj.activeSelf)
            {
                obj.SetActive( true );
            }
            else if (!show && obj.activeSelf)
            {
                obj.SetActive( false );
            }
        }
    }
}
