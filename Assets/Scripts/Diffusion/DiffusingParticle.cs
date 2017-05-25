using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Diffusion
{
	[RequireComponent( typeof(Rigidbody), typeof(Collider) )]
	public class DiffusingParticle : MonoBehaviour 
	{
		public Vector2 clockTimeBetweenImpulses = new Vector2( 0.1f, 0.5f );

		float lastTime = -1f;
		float timeInterval;
		Vector3 startPosition;
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

		MeshRenderer _meshRenderer;
		MeshRenderer meshRenderer
		{
			get {
				if (_meshRenderer == null)
				{
					_meshRenderer = GetComponent<MeshRenderer>();
				}
				return _meshRenderer;
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
			}
		}

		public void SetStartPosition ()
		{
			startPosition = transform.position;
		}

		float velocityMagnitude
		{
			get {
				return timeInterval * ParameterInput.Instance.velocityMultiplier 
					* Mathf.Sqrt( ParameterInput.Instance.diffusionCoefficient.value * ParameterInput.Instance.dTime.value );
			}
		}

		float angularVelocityMagnitude
		{
			get {
				return timeInterval * ParameterInput.Instance.angularVelocityMultiplier 
					* Mathf.Sqrt( ParameterInput.Instance.diffusionCoefficient.value * ParameterInput.Instance.dTime.value );
			}
		}

		public float displacement
		{
			get {
				return Vector3.Distance( startPosition, transform.position );
			}
		}

		void SetTimeInterval ()
		{
			timeInterval = Random.Range( clockTimeBetweenImpulses.x, clockTimeBetweenImpulses.y );
		}
	}
}