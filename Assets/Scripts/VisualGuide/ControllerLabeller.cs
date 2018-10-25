using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

[RequireComponent( typeof( ControllerInput ) )]
public class ControllerLabeller : MonoBehaviour 
{
    public GameObject scaleButtonLabelRight;
    public GameObject selectButtonLabelRight;
    public GameObject labelLineRight;
    public GameObject scaleButtonLabelLeft;
    public GameObject labelLineLeft;

    ControllerInput _controllers;
    ControllerInput controllers
    {
        get
        {
            if (_controllers == null)
            {
                _controllers = GetComponent<ControllerInput>();
            }
            return _controllers;
        }
    }

    void Update ()
    {
        UpdateButtonLabels();
    }

    void UpdateButtonLabels ()
    {
        if (!controllers.rightTriggerDown && !controllers.leftTriggerDown)
        {
            ShowObject( scaleButtonLabelRight, false );
            ShowObject( selectButtonLabelRight, true );
            ShowObject( labelLineRight, true );
            ShowObject( scaleButtonLabelLeft, true );
            ShowObject( labelLineLeft, true );
        }
        else if (controllers.rightTriggerDown && !controllers.leftTriggerDown)
        {
            ShowObject( scaleButtonLabelRight, false );
            ShowObject( selectButtonLabelRight, false );
            ShowObject( labelLineRight, false );
            ShowObject( scaleButtonLabelLeft, true );
            ShowObject( labelLineLeft, true );
        }
        else if (!controllers.rightTriggerDown && controllers.leftTriggerDown)
        {
            ShowObject( scaleButtonLabelRight, true );
            ShowObject( selectButtonLabelRight, false );
            ShowObject( labelLineRight, true );
            ShowObject( scaleButtonLabelLeft, false );
            ShowObject( labelLineLeft, false );
        }
        else
        {
            ShowObject( scaleButtonLabelRight, false );
            ShowObject( selectButtonLabelRight, false );
            ShowObject( labelLineRight, false );
            ShowObject( scaleButtonLabelLeft, false );
            ShowObject( labelLineLeft, false );
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
