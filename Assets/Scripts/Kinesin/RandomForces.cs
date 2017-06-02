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
					body.AddForce( Helpers.GetRandomVector( forceMagnitude ) );
					body.AddTorque( Helpers.GetRandomVector( torqueMagnitude ) );
				}

				SetTimeInterval();
				lastTime = Time.time;
			}
		}

		float forceMagnitude
		{
			get {
				// mass * time interval * multiplier * sqrt( diffusion coefficient * time step (ps) )
				float meanForce = body.mass * timeInterval * 2100f * Mathf.Sqrt( 0.005f * 1000f ); 
				return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / meanForce); // random exponential distribution
			}
		}

		float torqueMagnitude
		{
			get {
				// mass * time interval * multiplier * sqrt( diffusion coefficient * time step (ps) )
				float meanTorque = body.mass * timeInterval * 1500f * Mathf.Sqrt( 0.005f * 1000f );
				return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / meanTorque); // random exponential distribution
			}
		}

		void SetTimeInterval ()
		{
			timeInterval = Random.Range( minTimeBetweenImpulses, maxTimeBetweenImpulses );
		}
	}
}