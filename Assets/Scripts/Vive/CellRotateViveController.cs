using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CellRotateViveController : ViveController
{
	public Transform objectToRotate;
	public float multiplier = 0.1f;

	Vector3 thisLastPosition;

	public override void OnTriggerDown () 
	{
		thisLastPosition = transform.position;
		DoRotation();
	}

	public override void OnTriggerStay () 
	{
        Debug.Log("stay");
        DoRotation();
	}

	void DoRotation ()
	{
		Vector3 axis = Vector3.Cross( thisLastPosition, transform.position ).normalized;
		float angle = Vector3.Dot( thisLastPosition, transform.position );
        Debug.Log(angle);

        objectToRotate.RotateAround( objectToRotate.position, axis, multiplier * angle );

		thisLastPosition = transform.position;
	}
}
