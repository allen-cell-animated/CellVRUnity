using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public class RandomMotion : MonoBehaviour 
	{
		public float maxDisplacement = 10f;
		public float maxStepSize = 1f;

		Vector3 startLocalPosition;
		float minTimeBetweenImpulses = 0.03f;
		float maxTimeBetweenImpulses = 0.05f;
		float lastTime = -1f;
		float timeInterval;

		void Start ()
		{
			startLocalPosition = transform.localPosition;
			SetTimeInterval();
		}

		void Update () 
		{
			if (Time.time - lastTime > timeInterval)
			{
				Vector3 newPosition = transform.localPosition + Helpers.GetRandomVector( Random.Range( 0, maxStepSize ) );
				Vector3 startToNewPosition = newPosition - startLocalPosition;
				if (startToNewPosition.magnitude > maxDisplacement)
				{
					newPosition = startLocalPosition + maxDisplacement * Vector3.Normalize( startToNewPosition );
				}
				transform.localPosition = newPosition;

				SetTimeInterval();
				lastTime = Time.time;
			}
		}

		void SetTimeInterval ()
		{
			timeInterval = Random.Range( minTimeBetweenImpulses, maxTimeBetweenImpulses );
		}
	}
}