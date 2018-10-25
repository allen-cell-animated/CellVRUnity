using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerLabeller : MonoBehaviour 
{
    public GameObject scaleButtonLabelRight;
    public GameObject selectButtonLabelRight;
    public GameObject labelLineRight;
    public GameObject scaleButtonLabelLeft;
    public GameObject labelLineLeft;

    void Update ()
    {
        UpdateButtonLabels();
    }

    void UpdateButtonLabels ()
    {
        if (!ControllerInput.Instance.rightTriggerDown && !ControllerInput.Instance.leftTriggerDown)
        {
            ShowObject( scaleButtonLabelRight, false );
            ShowObject( selectButtonLabelRight, true );
            ShowObject( labelLineRight, true );
            ShowObject( scaleButtonLabelLeft, true );
            ShowObject( labelLineLeft, true );
        }
        else if (ControllerInput.Instance.rightTriggerDown && !ControllerInput.Instance.leftTriggerDown)
        {
            ShowObject( scaleButtonLabelRight, false );
            ShowObject( selectButtonLabelRight, false );
            ShowObject( labelLineRight, false );
            ShowObject( scaleButtonLabelLeft, true );
            ShowObject( labelLineLeft, true );
        }
        else if (!ControllerInput.Instance.rightTriggerDown && ControllerInput.Instance.leftTriggerDown)
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
