using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Throwable : MonoBehaviour 
{
    public Vector3 rotationOffsetAtTarget;

    VRTK_ControllerEvents touchingController;

    Rigidbody _body;
    Rigidbody body
    {
        get
        {
            if (_body == null)
            {
                _body = GetComponent<Rigidbody>();
            }
            return _body;
        }
    }

    Vector3 lastPosition;
    Vector3 velocity;

    void Update ()
    {
        velocity = transform.position - lastPosition;
        lastPosition = transform.position;
    }

    void OnEnable ()
    {
        if (ControllerInput.Instance.pointerLeft != null)
        {
            ControllerInput.Instance.pointerLeft.TriggerPressed += OnLeftControllerTriggerDown;
            ControllerInput.Instance.pointerLeft.TriggerReleased += OnLeftControllerTriggerUp;
        }
        if (ControllerInput.Instance.pointerRight != null)
        {
            ControllerInput.Instance.pointerRight.TriggerPressed += OnRightControllerTriggerDown;
            ControllerInput.Instance.pointerRight.TriggerReleased += OnRightControllerTriggerUp;
        }
    }

    void OnDisable ()
    {
        if (ControllerInput.Instance.pointerLeft != null)
        {
            ControllerInput.Instance.pointerLeft.TriggerPressed -= OnLeftControllerTriggerDown;
            ControllerInput.Instance.pointerLeft.TriggerReleased -= OnLeftControllerTriggerUp;
        }
        if (ControllerInput.Instance.pointerRight != null)
        {
            ControllerInput.Instance.pointerRight.TriggerPressed -= OnRightControllerTriggerDown;
            ControllerInput.Instance.pointerRight.TriggerReleased -= OnRightControllerTriggerUp;
        }
    }

    void OnRightControllerTriggerDown (object sender, ControllerInteractionEventArgs e)
    {
        if (touchingController != null && touchingController == ControllerInput.Instance.pointerRight)
        {
            OnTouchingControllerDown();
        }
    }

    void OnRightControllerTriggerUp (object sender, ControllerInteractionEventArgs e)
    {
        if (touchingController != null && touchingController == ControllerInput.Instance.pointerRight)
        {
            OnTouchingControllerUp();
        }
    }

    void OnLeftControllerTriggerDown (object sender, ControllerInteractionEventArgs e)
    {
        if (touchingController != null && touchingController == ControllerInput.Instance.pointerLeft)
        {
            OnTouchingControllerDown();
        }
    }

    void OnLeftControllerTriggerUp (object sender, ControllerInteractionEventArgs e)
    {
        if (touchingController != null && touchingController == ControllerInput.Instance.pointerLeft)
        {
            OnTouchingControllerUp();
        }
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.tag == "GameController")
        {
            touchingController = other.gameObject.GetComponentInParent<VRTK_ControllerEvents>();
            SetHighlight( true );
        }
    }

    void OnTriggerExit (Collider other)
    {
        if (other.tag == "GameController")
        {
            touchingController = null;
            SetHighlight( false );
        }
    }

    void SetHighlight (bool on)
    {
        // TODO
    }

    void OnTouchingControllerDown ()
    {
        body.isKinematic = true;
        transform.SetParent( touchingController.transform );
    }

    void OnTouchingControllerUp ()
    {
        transform.SetParent( MitosisGameManager.Instance.transform );
        body.isKinematic = false;
        body.AddForce( 5000f * velocity );
    }

    void OnCollisionEnter (Collision collision)
    {
        if (collision.gameObject.tag == "Target")
        {
            if (collision.gameObject.name.Contains( name.Substring( 0, name.Length - 7 ) ))
            {
                body.isKinematic = true;
                transform.position = collision.transform.position;
                transform.rotation = collision.transform.rotation * Quaternion.Euler( rotationOffsetAtTarget );
            }
            else
            {
                collision.gameObject.GetComponent<Animator>().SetTrigger( "Fail" );
            }
        }
    }
}
