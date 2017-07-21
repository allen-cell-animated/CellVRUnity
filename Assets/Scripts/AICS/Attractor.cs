using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	[RequireComponent( typeof(Rigidbody) )]
	public class Attractor : MonoBehaviour 
	{
		public Transform target;
		public bool goToPosition = false;
		public float attractiveForce = 30f;
		public float timeInterval = -1f;

		float lastTime = -1f;
		Vector3 goalPosition;

		public bool attracting 
		{
			get
			{
				return (goToPosition || target != null);
			}
		}

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
			if (attracting && (timeInterval < 0 || Time.time - lastTime > timeInterval))
			{
				ApplyAttractiveForce();
				lastTime = Time.time;
			}
		}

		public void GoToPosition (Vector3 _position, float _attractiveForce = -1f)
		{
			goToPosition = true;
			goalPosition = _position;
			if (attractiveForce >= 0)
			{
				attractiveForce = _attractiveForce;
			}
		}

		public void GoToTransform (Transform _transform, float _attractiveForce = -1f)
		{
			goToPosition = false;
			target = _transform;
			if (attractiveForce >= 0)
			{
				attractiveForce = _attractiveForce;
			}
		}

		public void Stop ()
		{
			target = null;
			goToPosition = false;
		}

		void ApplyAttractiveForce ()
		{
			if (goToPosition)
			{
				body.AddForce( attractiveForce * Vector3.Normalize( goalPosition - transform.position ) );
			}
			else 
			{
				body.AddForce( attractiveForce * Vector3.Normalize( target.position - transform.position ) );
			}
		}
	}
}
