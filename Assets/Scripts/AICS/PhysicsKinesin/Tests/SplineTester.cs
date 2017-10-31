using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MT;

public enum SpecialVector
{
	tangent,
	normal
}

public class SplineTester : MonoBehaviour 
{
	public SpecialVector lookDirection;
	public Spline spline;
	public float speed = 0.1f;

	float t = 0;

	void Update () 
	{
		t += speed * Time.deltaTime;
		if (t > 1f)
		{
			t = 0;
		}
		PositionAlongSpline();
	}

	void PositionAlongSpline ()
	{
		transform.position = spline.GetPosition( t );
		if (lookDirection == SpecialVector.tangent)
		{
			transform.LookAt( transform.position + spline.GetTangent( t ), spline.GetNormal( t ) );
		}
		else if (lookDirection == SpecialVector.normal)
		{
			transform.LookAt( transform.position + spline.GetNormal( t ), spline.GetTangent( t ) );
		}
	}
}
