using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerLabeller : MonoBehaviour 
{
    public GameObject scaleButtonLabelRight;
    public GameObject grabButtonLabelRight;
    public GameObject selectButtonLabelRight;

    public GameObject scaleButtonLabelLeft;
    public GameObject grabButtonLabelLeft;
    public GameObject backButtonLabelLeft;

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

            ShowObject( selectButtonLabelRight, !ControllerInput.Instance.rightTriggerDown );

            ShowObject( grabButtonLabelRight, false );
            ShowObject( grabButtonLabelLeft, false );
            ShowObject( backButtonLabelLeft, false );
        }
        else
        {
            ShowObject( grabButtonLabelRight, !ControllerInput.Instance.rightTriggerDown );
            ShowObject( grabButtonLabelLeft, !ControllerInput.Instance.leftTriggerDown );

            ShowObject( backButtonLabelLeft, true );

            ShowObject( scaleButtonLabelRight, false );
            ShowObject( scaleButtonLabelLeft, false );
            ShowObject( selectButtonLabelRight, false );
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
