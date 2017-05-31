using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	[RequireComponent( typeof(Rigidbody) )]
	public class Attractor : MonoBehaviour 
	{
		public Transform target;
		public float attractiveForce = 30f;
		public float timeInterval = -1f;

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
			if (target != null && (timeInterval < 0 || Time.time - lastTime > timeInterval))
			{
				ApplyAttractiveForce();
				lastTime = Time.time;
			}
		}

		void ApplyAttractiveForce ()
		{
			body.AddForce( attractiveForce * Vector3.Normalize( target.position - transform.position ) );
		}
	}
}
