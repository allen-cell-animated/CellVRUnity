using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MotorProteins;

namespace AICS.PhysicsKinesin
{
	public class RandomForces : MonoBehaviour 
	{
		public bool addForce = true;
		public bool addTorque = true;
		public float nanosecondsPerStep = 500000f;

		float lastNanoseconds = 0;
		float lastTime = 0;
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

		void FixedUpdate () 
		{
			float t = (MolecularEnvironment.Instance.nanosecondsSinceStart - lastNanoseconds) / nanosecondsPerStep;
			if (body != null && (MolecularEnvironment.Instance.nanosecondsSinceStart == 0 || t >= 1f))
			{
				timeInterval = Time.time - lastTime;

				body.velocity = body.angularVelocity = Vector3.zero;
				if (addForce)
				{
					body.AddForce( Helpers.GetRandomVector( forceMagnitude ) );
				}
				if (addTorque)
				{
					body.AddTorque( Helpers.GetRandomVector( torqueMagnitude ) );
				}

				lastNanoseconds = MolecularEnvironment.Instance.nanosecondsSinceStart;
				lastTime = Time.time;
			}
		}

		float forceMagnitude
		{
			get {
				// mass * time interval * multiplier * sqrt( diffusion coefficient * time step (ps) )
				float meanForce = Mathf.Min( 100f, body.mass ) * timeInterval * 2100f 
					* Mathf.Sqrt( 20f * 1E-4f * MolecularEnvironment.Instance.nanosecondsPerStep ); 
				return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / meanForce); // random exponential distribution
			}
		}

		float torqueMagnitude
		{
			get {
				// mass * time interval * multiplier * sqrt( diffusion coefficient * time step (ps) )
				float meanTorque = Mathf.Min( 100f, body.mass ) * timeInterval * 1500f 
					* Mathf.Sqrt( 20f * 1E-4f * MolecularEnvironment.Instance.nanosecondsPerStep ); 
				return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / meanTorque); // random exponential distribution
			}
		}

		public void DoReset ()
		{
			lastNanoseconds = 0;
		}
	}
}