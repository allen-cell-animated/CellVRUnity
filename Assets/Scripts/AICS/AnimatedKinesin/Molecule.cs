using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AnimatedKinesin
{
	public abstract class Molecule : MonoBehaviour 
	{
		public float maxDistanceFromParent = 8f;
		public float meanStepSize = 0.2f;
		public float meanRotation = 1f;

		Rigidbody _body;
		protected Rigidbody body 
		{
			get
			{
				if (_body == null)
				{
					_body = GetComponent<Rigidbody>();
					_body.useGravity = false;
					_body.isKinematic = true;
				}
				return _body;
			}
		}

		protected void Rotate ()
		{
			transform.rotation *= Quaternion.Euler( Helpers.GetRandomVector( SampleExponentialDistribution( meanRotation ) ) );
		}

		protected void Move () 
		{
			Vector3 moveStep = Helpers.GetRandomVector( SampleExponentialDistribution( meanStepSize ) );
			if (!WillCollide( moveStep ))
			{
				if (transform.parent == null)
				{
					transform.position += moveStep;
				}
				else if (Vector3.Magnitude( transform.localPosition + moveStep ) <= maxDistanceFromParent)
				{
					transform.position += moveStep;
				}
			}
		}

		protected abstract bool WillCollide (Vector3 moveStep);

		float SampleExponentialDistribution (float mean)
		{
			return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / mean);
		}
	}
}