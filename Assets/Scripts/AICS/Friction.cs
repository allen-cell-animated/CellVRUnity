using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	[RequireComponent( typeof(Rigidbody) )]
	public class Friction : MonoBehaviour 
	{
		public float magnitude = -20f;
		public float timeInterval = 0.1f;

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

		void Update () 
		{
			if (Time.time - lastTime > timeInterval)
			{
				ApplyFriction();
				lastTime = Time.time;
			}
		}

		void ApplyFriction ()
		{
			body.AddForce( magnitude * body.velocity );
		}
	}
}