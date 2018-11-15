using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Transformer : MonoBehaviour 
{
    public bool transforming;
    public bool canScale = true;
    public bool canRotate = true;
    public LineRenderer scaleLine;

    bool wasTransforming;
    Vector3 startScale;
    float startControllerDistance;
    Quaternion startRotation;
    Vector3 startControllerVector;
    Vector3 startPositiveVector;
    Vector3 minScale = new Vector3( 0.2f, 0.2f, 0.2f );
    Vector3 maxScale = new Vector3( 3f, 3f, 3f );
    Vector3[] linePoints = new Vector3[2];

    void Update ()
    {
        UpdateTransforming();
    }

    // TRANSLATING --------------------------------------------------------------------------------------------------

    void UpdateTransforming ()
    {
        if (ControllerInput.Instance.rightTriggerDown && ControllerInput.Instance.leftTriggerDown)
        {
            if (!transforming)
            {
                transforming = wasTransforming = true;
                ControllerInput.Instance.ToggleLaserRenderer( false );
                StartScaling();
                StartRotating();
            }
            else
            {
                UpdateScale();
                UpdateRotation();
            }
            ToggleLine( true );
        }
        else if (transforming)
        {
            ToggleLine( false );
            ControllerInput.Instance.ToggleLaserRenderer( true );
            transforming = false;
        }
    }

    void StartScaling ()
    {
        if (canScale)
        {
            startScale = transform.localScale;
            startControllerDistance = Vector3.Distance( ControllerInput.Instance.pointerRight.transform.position, ControllerInput.Instance.pointerLeft.transform.position );
        }
    }

    void UpdateScale ()
    {
        if (canScale)
        {
            float scale = Vector3.Distance( ControllerInput.Instance.pointerRight.transform.position, ControllerInput.Instance.pointerLeft.transform.position ) / startControllerDistance;

            transform.localScale = ClampScale( scale * startScale );
        }
    }

    Vector3 ClampScale (Vector3 _scale)
    {
        if (_scale.magnitude > maxScale.magnitude)
        {
            return maxScale;
        }
        else if (_scale.magnitude < minScale.magnitude)
        {
            return minScale;
        }
        else
        {
            return _scale;
        }
    }

    void StartRotating ()
    {
        if (canRotate)
        {
            startRotation = transform.localRotation;
            startControllerVector = ControllerInput.Instance.pointerRight.transform.position - ControllerInput.Instance.pointerLeft.transform.position;
            startControllerVector.y = 0;
            startPositiveVector = Vector3.Cross( startControllerVector, Vector3.up );
        }
    }

    void UpdateRotation ()
    {
        if (canRotate)
        {
            Vector3 controllerVector = ControllerInput.Instance.pointerRight.transform.position - ControllerInput.Instance.pointerLeft.transform.position;
            controllerVector.y = 0;
            float direction = GetArcCosineDegrees( Vector3.Dot( startPositiveVector.normalized, controllerVector.normalized ) ) >= 90f ? 1f : -1f;
            float dAngle = direction * GetArcCosineDegrees( Vector3.Dot( startControllerVector.normalized, controllerVector.normalized ) );

            transform.localRotation = startRotation * Quaternion.AngleAxis( dAngle, Vector3.up );
        }
    }

    float GetArcCosineDegrees (float cosine)
    {
        if (cosine > 1f - float.Epsilon)
        {
            return 0;
        }
        if (cosine < -1f + float.Epsilon)
        {
            return 180f;
        }
        return Mathf.Acos( cosine ) * Mathf.Rad2Deg;
    }

    void ToggleLine (bool _active)
    {
        if (_active)
        {
            if (!scaleLine.gameObject.activeSelf)
            {
                scaleLine.gameObject.SetActive( true );
            }
            linePoints[0] = ControllerInput.Instance.pointerRight.transform.position;
            linePoints[1] = ControllerInput.Instance.pointerLeft.transform.position;
            scaleLine.SetPositions( linePoints );
        }
        else
        {
            scaleLine.gameObject.SetActive( false );
        }
    }
}
