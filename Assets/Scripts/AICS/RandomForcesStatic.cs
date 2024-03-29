﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public class RandomForcesStatic : MonoBehaviour 
	{
		public bool addForce = true;
		public bool addTorque = true;

		float minTimeBetweenImpulses = 0.05f;
		float maxTimeBetweenImpulses = 0.1f;
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
				if (addForce)
				{
					body.AddForce( Helpers.GetRandomVector( forceMagnitude ) );
				}
				if (addTorque)
				{
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
				float meanForce = body.mass * timeInterval * 2100f * Mathf.Sqrt( 20f * 1E-4f * 10000f ); 
				return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / meanForce); // random exponential distribution
			}
		}

		float torqueMagnitude
		{
			get {
				// mass * time interval * multiplier * sqrt( diffusion coefficient * time step (ps) )
				float meanTorque = body.mass * timeInterval * 1500f * Mathf.Sqrt( 20f * 1E-4f * 10000f ); 
				return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / meanTorque); // random exponential distribution
			}
		}

		void SetTimeInterval ()
		{
			timeInterval = Random.Range( minTimeBetweenImpulses, maxTimeBetweenImpulses );
		}
	}
}