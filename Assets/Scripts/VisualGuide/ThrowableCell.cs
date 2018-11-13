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
    Vector3 lastPosition;
    Vector3 velocity;

    MitosisGameManager _gameManager;
    MitosisGameManager gameManager
    {
        get
        {
            if (_gameManager == null)
            {
                _gameManager = GetComponentInParent<MitosisGameManager>();
            }
            return _gameManager;
        }
    }

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

    protected override void Update ()
    {
        base.Update();

        velocity = transform.position - lastPosition;
        lastPosition = transform.position;
    }

    void OnCollisionEnter (Collision collision)
    {
        if (collision.gameObject.tag == "Target")
        {
            if (collision.gameObject.name.Contains( name.Substring( 0, name.Length - 7 ) ))
            {
                BindToTarget( collision.gameObject );
            }
            else
            {
                BounceOffTarget( collision.gameObject );
            }
        }
    }

    public override void Grabbed (VRTK_InteractGrab currentGrabbingObject = null)
    {
        base.Grabbed( currentGrabbingObject );
        ReleaseFromTarget( false );
    }

    public override void Ungrabbed (VRTK_InteractGrab previousGrabbingObject = null)
    {
        base.Ungrabbed( previousGrabbingObject );
        ReleaseFromTarget( false );
    }

    void BindToTarget (GameObject target)
    {
        if (gameManager != null)
        {
            boundToTarget = true;
            body.isKinematic = true;
            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation * Quaternion.Euler(rotationOffsetAtTarget);
            attachedTargetRenderer = target.GetComponentInChildren<SpriteRenderer>();
            attachedTargetRenderer.enabled = false;
            gameManager.RecordCorrectHit();
        }
    }

    void BounceOffTarget (GameObject target)
    {
        target.GetComponent<Animator>().SetTrigger( "Fail" );
    }

    public void ReleaseFromTarget (bool resetVelocity)
    {
        body.isKinematic = boundToTarget = false;
        if (attachedTargetRenderer != null)
        {
            attachedTargetRenderer.enabled = true;
            attachedTargetRenderer = null;
        }
        if (resetVelocity)
        {
            body.velocity = Vector3.zero;
        }
    }
}
