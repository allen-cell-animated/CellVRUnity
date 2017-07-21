using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Necklinker
{
	public class RandomForces : MonoBehaviour 
	{
		public bool addForce = true;
		public bool addTorque = true;

		float lastTime = -1f;

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
			if (body != null && Time.time - lastTime > (1f / NecklinkerParameterInput.Instance.forceFrequency.value))
			{
				body.velocity = body.angularVelocity = Vector3.zero;
				if (addForce)
				{
					body.AddForce( Helpers.GetRandomVector( forceMagnitude ) );
				}
				if (addTorque)
				{
					body.AddTorque( Helpers.GetRandomVector( torqueMagnitude ) );
				}

				lastTime = Time.time;
			}
		}

		float forceMagnitude
		{
			get {
				// mass * time interval * multiplier * sqrt( diffusion coefficient * time step (ps) )
				float meanForce = body.mass * (1f / NecklinkerParameterInput.Instance.forceFrequency.value) * 2100f 
					* Mathf.Sqrt( 20f * 1E-4f * NecklinkerParameterInput.Instance.dTime.value ); 
				return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / meanForce); // random exponential distribution
			}
		}

		float torqueMagnitude
		{
			get {
				// mass * time interval * multiplier * sqrt( diffusion coefficient * time step (ps) )
				float meanTorque = body.mass * (1f / NecklinkerParameterInput.Instance.forceFrequency.value) * 1500f 
					* Mathf.Sqrt( 20f * 1E-4f * NecklinkerParameterInput.Instance.dTime.value ); 
				return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / meanTorque); // random exponential distribution
			}
		}
	}
}