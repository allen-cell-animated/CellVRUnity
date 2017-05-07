using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour 
{
	public Transform objectToRotate;
	public float multiplier = 0.1f;

	Vector3 thisLastPosition;

	void Start ()
	{
		thisLastPosition = transform.position;
	}

	void Update () 
	{
//		if (Input.GetKey (KeyCode.R)) 
//		{
			DoRotation();
//		}
	}

	void DoRotation ()
	{
		Vector3 axis = Vector3.Cross( thisLastPosition, transform.position ).normalized;
		float angle = Vector3.Dot( thisLastPosition, transform.position );

		objectToRotate.RotateAround( objectToRotate.position, axis, angle * multiplier );

		thisLastPosition = transform.position;
	}
}
