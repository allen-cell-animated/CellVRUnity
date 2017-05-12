using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	[RequireComponent( typeof(Rigidbody) )]
	public class RandomForces : MonoBehaviour 
	{
		public float minForceMagnitude = 50f;
		public float maxForceMagnitude = 100f;
		public float minTimeBetweenImpulses = 0.1f;
		public float maxTimeBetweenImpulses = 0.2f;

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

		void Update () 
		{
			if (Time.time - lastTime > timeInterval)
			{
				body.velocity = body.angularVelocity = Vector3.zero;
				body.AddForce( GetRandomVector() );

				SetTimeInterval();
				lastTime = Time.time;
			}
		}

		void SetTimeInterval ()
		{
			timeInterval = Random.Range( minTimeBetweenImpulses, maxTimeBetweenImpulses );
		}

		Vector3 GetRandomVector ()
		{
			Vector3 random = new Vector3( Random.Range( -1f, 1f ), Random.Range( -1f, 1f ), Random.Range( -1f, 1f ) );
			return Random.Range( minForceMagnitude, maxForceMagnitude ) * Vector3.Normalize( random );
		}
	}
}