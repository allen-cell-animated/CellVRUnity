using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSweep : MonoBehaviour 
{
	Rigidbody _body;
	Rigidbody body 
	{
		get
		{
			if (_body == null)
			{
				_body = GetComponent<Rigidbody>();
				_body.useGravity = false;
				_body.isKinematic = true;
			}
			return _body;
		}
	}

	void Update () 
	{
		if (Input.GetKeyDown( KeyCode.Q ))
		{
			SweepTest( 0.1f );
		}
		if (Input.GetKeyDown( KeyCode.W ))
		{
			SweepTest( 0.4f );
		}
		if (Input.GetKeyDown( KeyCode.E ))
		{
			SweepTest( 1f );
		}
	}

	void SweepTest (float distance)
	{
		RaycastHit[] hits = body.SweepTestAll( Vector3.forward, distance, UnityEngine.QueryTriggerInteraction.Collide );
		if (hits.Length > 0)
		{
			Debug.Log( "HIT" );
		}
		else
		{
			Debug.Log( "missed" );
		}
	}
}
