using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ThrowableCell : VRTK_InteractableObject 
{
    [Header("Throwable Cell Settings")]

    public float boundsRadius;
    public Vector3 rotationOffsetAtTarget;
    public Vector3 alignedMitosisPosition;
    public Vector3 alignedMitosisRotation;
    public float alignedMitosisScale;
    public Target attachedTarget;

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

    Mover _mover;
    Mover mover
    {
        get
        {
            if (_mover == null)
            {
                _mover = gameObject.AddComponent<Mover>();
            }
            return _mover;
        }
    }

    Rotator _rotator;
    Rotator rotator
    {
        get
        {
            if (_rotator == null)
            {
                _rotator = gameObject.AddComponent<Rotator>();
            }
            return _rotator;
        }
    }

    Scaler _scaler;
    Scaler scaler
    {
        get
        {
            if (_scaler == null)
            {
                _scaler = gameObject.AddComponent<Scaler>();
            }
            return _scaler;
        }
    }

    Colorer _colorer;
    public Colorer colorer
    {
        get
        {
            if (_colorer == null)
            {
                _colorer = GetComponentInChildren<Colorer>();
            }
            return _colorer;
        }
    }

    public bool isMoving
    {
        get
        {
            return !(body.isKinematic || body.velocity.magnitude < 0.1f);
        }
    }

    void OnCollisionEnter (Collision collision)
    {
        Target target = collision.gameObject.GetComponent<Target>();
        if (target != null)
        {
            if (target.goalName.Contains( name.Substring( 0, name.Length - 11 ) ))
            {
                BindToTarget( target );
            }
            else
            {
                target.Bounce();
            }
        }
    }

    public override void Grabbed (VRTK_InteractGrab currentGrabbingObject = null)
    {
        base.Grabbed( currentGrabbingObject );
        ReleaseFromTarget();
    }

    public override void Ungrabbed (VRTK_InteractGrab previousGrabbingObject = null)
    {
        base.Ungrabbed( previousGrabbingObject );
        ReleaseFromTarget();
    }

    void BindToTarget (Target target)
    {
        if (gameManager != null)
        {
            attachedTarget = target;
            attachedTarget.Bind();

            body.isKinematic = true;
            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation * Quaternion.Euler(rotationOffsetAtTarget);

            gameManager.RecordCorrectHit();
        }
    }

    public void ReleaseFromTarget (bool resetVelocity = false)
    {
        if (attachedTarget != null)
        {
            gameManager.RemoveCorrectHit();
            attachedTarget.Release();
            attachedTarget = null;
        }

        body.isKinematic = false;
        if (resetVelocity)
        {
            body.velocity = Vector3.zero;
        }
    }

    public void AnimateSuccess ()
    {
        mover.MoveToOverDuration( alignedMitosisPosition, 1f, gameManager.AnimateNextPhase );
        rotator.RotateToOverDuration( Quaternion.Euler( alignedMitosisRotation ), 1f );
        scaler.ScaleOverDuration( alignedMitosisScale, 1f );
    }
}
