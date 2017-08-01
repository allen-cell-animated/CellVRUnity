using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AnimatedKinesin
{
	public abstract class Molecule : MonoBehaviour 
	{
		public float maxDistanceFromParent = 6f;
		public float meanStepSize = 0.2f;
		public float meanRotation = 5f;

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

		public void Rotate ()
		{
			transform.rotation *= Quaternion.Euler( Helpers.GetRandomVector( SampleExponentialDistribution( meanRotation ) ) );
		}

		public void Move () 
		{
			float stepSize = SampleExponentialDistribution( meanStepSize );
			Vector3 moveStep = Helpers.GetRandomVector( stepSize );
			if (!WillCollide( moveStep ))
			{
				if (transform.parent == null || WithinLeash( moveStep ))
				{
					transform.position += moveStep;
				}
				else if (transform.parent != null)
				{
					moveStep = stepSize * towardLeashDirection;
					if (!WillCollide( moveStep ))
					{
						transform.position += moveStep;
					}
				}
			}
		}

		protected abstract bool WillCollide (Vector3 moveStep);

		protected abstract bool WithinLeash (Vector3 moveStep);

		protected abstract Vector3 towardLeashDirection
		{
			get;
		}

		public void Jitter () 
		{
			transform.position += Helpers.GetRandomVector( SampleExponentialDistribution( 0.02f ) );
		}

		float SampleExponentialDistribution (float mean)
		{
			return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / mean);
		}
	}
}