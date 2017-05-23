using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	[RequireComponent( typeof(Rigidbody) )]
	public class RandomForces : MonoBehaviour 
	{
		public Vector2 clockTimeBetweenImpulses;
		public float displacement;

		float lastTime = -1f;
		float timeInterval;
		Vector3 startPosition;
		bool startPositionSet;
		int samples;

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
				SetTimeInterval();

				body.velocity = Helpers.GetRandomVector( velocityMagnitude );
				body.angularVelocity = Helpers.GetRandomVector( angularVelocityMagnitude );

				lastTime = Time.time;
				CalculateDisplacement();
			}

			if (Time.time > 1f && !startPositionSet)
			{
				startPosition = transform.position;
				startPositionSet = true;
			}
		}

		float velocityMagnitude
		{
			get {
				return ParameterInput.Instance.velocityMultiplier 
					* Mathf.Sqrt( 6f * 0.01f * ParameterInput.Instance.diffusionCoefficient.value * dTimePS ) / timeInterval;
			}
		}

		float angularVelocityMagnitude
		{
			get {
				return ParameterInput.Instance.angularVelocityMultiplier 
					* Mathf.Sqrt( 6f * 0.01f * ParameterInput.Instance.diffusionCoefficient.value * dTimePS ) / timeInterval;
			}
		}

		float dTimePS // time interval in picoseconds
		{
			get {
				return timeInterval * ParameterInput.Instance.dTime.value;
			}
		}

		public void CalculateDisplacement ()
		{
			displacement = Vector3.Distance( startPosition, transform.position );
		}

		void SetTimeInterval ()
		{
			timeInterval = Random.Range( clockTimeBetweenImpulses.x, clockTimeBetweenImpulses.y );
		}
	}
}