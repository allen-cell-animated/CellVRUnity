using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStepsPerFrame : MonoBehaviour 
{
	public int stepsPerFrame = 11;
	public float radius;
	public Transform nearbyObject;

	Vector3 startPosition;
	bool atStartPosition = true;
	int t = 0;

	void Update ()
	{
		int c = 0;
		t = 0;
		for (int i = 0; i < stepsPerFrame; i++)
		{
			TogglePosition();
			if (Colliding())
			{
				c++;
			}
		}
		Debug.Log( c + " / " + t );
	}

	void TogglePosition ()
	{
		if (atStartPosition)
		{
			transform.position = nearbyObject.transform.position;
			t++;
		}
		else 
		{
			transform.position = startPosition;
		}
		atStartPosition = !atStartPosition;
	}

	bool Colliding ()
	{
		return Vector3.Distance( nearbyObject.position, transform.position ) <= 2f * radius;
	}
}
