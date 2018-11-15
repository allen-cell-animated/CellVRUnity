using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaler : MonoBehaviour 
{
    public bool scaling;

    float startScale;
    float goalScale;
    float t;
    float speed;
    CompletionDelegate callback;

    public void ScaleOverDuration (float _goalScale, float _duration, CompletionDelegate _callback = null)
    {
        callback = _callback;
        speed = 1f / _duration;

        Scale( _goalScale );
    }

    public void ScaleWithSpeed (float _goalScale, float _speed, CompletionDelegate _callback = null)
    {
        callback = _callback;
        float distance = _goalScale - transform.localScale.x;
        speed = 1f / (distance / _speed);

        Scale( _goalScale );
    }

    void Scale (float _goalScale)
    {
        startScale = transform.localScale.x;
        goalScale = _goalScale;
        t = 0;
        scaling = true;
    }

    void Update () 
    {
        if (scaling)
        {
            t += speed * Time.deltaTime;
            if (t >= 1f)
            {
                t = 1f;
                scaling = false;
                if (callback != null)
                {
                    callback();
                }
            }

            transform.localScale = ( startScale + t * (goalScale - startScale) ) * Vector3.one;
        }
    }
}