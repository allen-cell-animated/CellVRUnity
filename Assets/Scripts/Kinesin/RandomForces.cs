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
				return body.mass * timeInterval * 2100f * 0.005f * 1000f; // mass * time interval * multiplier * diffusion coefficient * time step (ps)
			}
		}

		float torqueMagnitude
		{
			get {
				return body.mass * timeInterval * 1500f * 0.005f * 1000f; // mass * time interval * multiplier * diffusion coefficient * time step (ps)
			}
		}

		void SetTimeInterval ()
		{
			timeInterval = Random.Range( minTimeBetweenImpulses, maxTimeBetweenImpulses );
		}
	}
}