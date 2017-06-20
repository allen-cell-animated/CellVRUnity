using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Kinesin;

public enum SpecialVector
{
	tangent,
	normal
}

public class SplineTester : MonoBehaviour 
{
	public SpecialVector lookDirection;
	public Spline spline;
	public float speed = 1f;

	float t = 0;
	float lastTime = -1;

	void Update () 
	{
		if (Time.time - lastTime > 0.1f)
		{
			t += speed / 100f;
			if (t > 1f)
			{
				t = 0;
			}
			PositionAlongSpline();
			lastTime = Time.time;
		}
	}

	void PositionAlongSpline ()
	{
		transform.position = spline.GetPoint( t );
		if (lookDirection == SpecialVector.tangent)
		{
			transform.LookAt( transform.position + spline.GetTangent( t ) );
		}
		else if (lookDirection == SpecialVector.normal)
		{
			transform.LookAt( transform.position + spline.GetNormal( t ) );
		}
	}
}
