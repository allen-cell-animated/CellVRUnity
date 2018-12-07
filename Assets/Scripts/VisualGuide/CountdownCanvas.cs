using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownCanvas : MonoBehaviour 
{
    public Animator numbers;

    Vector3 offsetFromCamera = new Vector3( 0, 0.5f, 3f );

    Transform _calculator;
    Transform calculator
    {
        get
        {
            if (_calculator == null)
            {
                _calculator = new GameObject( "Calculator" ).transform;
            }
            return _calculator;
        }
    }

    public void StartCountdown ()
    {
        numbers.SetTrigger( "play" );
        SetPosition();
    }

    public void FinishCountdown ()
    {
        UIManager.Instance.StartTimer();
        gameObject.SetActive( false );
    }

    void SetPosition ()
    {
        calculator.position = Camera.main.transform.position;
        calculator.rotation = Quaternion.Euler( 0, Camera.main.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z );

        Vector3 position = calculator.TransformPoint( offsetFromCamera );
        transform.position = new Vector3( position.x, 1f, position.z );

        transform.LookAt( transform.position + (transform.position - Camera.main.transform.position) );
        transform.rotation = Quaternion.Euler( 0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z );
    }
}
