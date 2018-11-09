using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ThrowableCell : VRTK_InteractableObject 
{
    [Header("Throwable Cell Settings")]

    public bool boundToTarget;
    public float boundsRadius;
    public Vector3 rotationOffsetAtTarget;

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

    protected override void Update ()
    {
        base.Update();

        velocity = transform.position - lastPosition;
        lastPosition = transform.position;
    }

    public override void Grabbed (VRTK_InteractGrab currentGrabbingObject = null)
    {
        base.Grabbed( currentGrabbingObject );
        boundToTarget = false;
        if (attachedTargetRenderer != null)
        {
            attachedTargetRenderer.enabled = true;
            attachedTargetRenderer = null;
        }
    }

    public override void Ungrabbed (VRTK_InteractGrab previousGrabbingObject = null)
    {
        base.Ungrabbed( previousGrabbingObject );
        Release( false );
        body.AddForce( 5000f * velocity );
    }

    public void Release (bool resetVelocity)
    {
        transform.SetParent( MitosisGameManager.Instance.transform );
        body.isKinematic = false;
        boundToTarget = false;
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
                boundToTarget = true;
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
