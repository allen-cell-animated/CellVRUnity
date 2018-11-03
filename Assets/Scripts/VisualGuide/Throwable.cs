using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Throwable : MonoBehaviour 
{
    public bool pickedUp;
    public bool bound;
    public float boundsRadius;
    public Vector3 rotationOffsetAtTarget;

    VRTK_ControllerEvents touchingController;
    SpriteRenderer attachedTargetRenderer;

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

    public bool isMoving
    {
        get
        {
            return !(body.isKinematic || body.velocity.magnitude < 0.1f);
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
        if (other.tag == "GameController" && !pickedUp)
        {
            touchingController = other.gameObject.GetComponentInParent<VRTK_ControllerEvents>();
            SetHighlight( true );
        }
    }

    void OnTriggerExit (Collider other)
    {
        if (other.tag == "GameController" && !pickedUp)
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
        bound = false;
        pickedUp = true;
        body.isKinematic = true;
        transform.SetParent( touchingController.transform );
        if (attachedTargetRenderer != null)
        {
            attachedTargetRenderer.enabled = true;
            attachedTargetRenderer = null;
        }
    }

    void OnTouchingControllerUp ()
    {
        Release( false );
        body.AddForce( 5000f * velocity );
    }

    public void Release (bool resetVelocity)
    {
        transform.SetParent( MitosisGameManager.Instance.transform );
        body.isKinematic = false;
        pickedUp = bound = false;
        if (resetVelocity)
        {
            body.velocity = Vector3.zero;
        }
    }

    void OnCollisionEnter (Collision collision)
    {
        if (collision.gameObject.tag == "Target")
        {
            if (collision.gameObject.name.Contains( name.Substring( 0, name.Length - 7 ) ))
            {
                bound = true;
                body.isKinematic = true;
                transform.position = collision.transform.position;
                transform.rotation = collision.transform.rotation * Quaternion.Euler( rotationOffsetAtTarget );
                attachedTargetRenderer = collision.gameObject.GetComponentInChildren<SpriteRenderer>();
                attachedTargetRenderer.enabled = false;
            }
            else
            {
                collision.gameObject.GetComponent<Animator>().SetTrigger( "Fail" );
            }
        }
    }
}
