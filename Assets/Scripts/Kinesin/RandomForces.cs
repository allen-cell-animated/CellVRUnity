using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class RandomForces : MonoBehaviour 
	{
		public bool addForces = true;
		public float minForceMagnitude = 50f;
		public float maxForceMagnitude = 100f;
		public float minTimeBetweenImpulses = 0.1f;
		public float maxTimeBetweenImpulses = 0.5f;

		float lastTime = -1f;
		float timeInterval;

		Rigidbody _rigidbody;
		Rigidbody body
		{
			get {
				if (_rigidbody == null)
				{
					_rigidbody = GetComponent<Rigidbody>();
				}
				return _rigidbody;
			}
		}

		void Start ()
		{
			SetTimeInterval();
		}

		void FixedUpdate () 
		{
			if (body != null && Time.time - lastTime > timeInterval)
			{
				body.velocity = body.angularVelocity = Vector3.zero;
				if (addForces)
				{
					body.AddForce( Helpers.GetRandomVector( Random.Range( minForceMagnitude, maxForceMagnitude ) ) );
				}

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