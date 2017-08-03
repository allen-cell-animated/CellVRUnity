using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.AnimatedKinesin
{
	public abstract class Molecule : MonoBehaviour 
	{
		public float radius;
		public float minDistanceFromParent = 2f;
		public float maxDistanceFromParent = 6f;
		public float meanStepSize = 0.2f;
		public float meanRotation = 5f;

		Kinesin _kinesin;
		protected Kinesin kinesin
		{
			get
			{
				if (_kinesin == null)
				{
					_kinesin = GameObject.FindObjectOfType<Kinesin>();
				}
				return _kinesin;
			}
		}

		protected abstract bool canMove
		{
			get;
		}

		public abstract void DoRandomWalk ();

		public void Rotate ()
		{
			transform.rotation *= Quaternion.Euler( Helpers.GetRandomVector( SampleExponentialDistribution( meanRotation ) ) );
		}

		public bool Move () 
		{
			float stepSize = SampleExponentialDistribution( meanStepSize );
			Vector3 moveStep = Helpers.GetRandomVector( stepSize );
			return MoveIfValid( moveStep );
		}

		protected bool MoveIfValid (Vector3 moveStep)
		{
			if (!WillCollide( moveStep ))
			{
				if (transform.parent == null || WithinLeash( moveStep ))
				{
					transform.position += moveStep;
					return true;
				}
			}
			return false;
		}

		public bool WillCollide (Vector3 moveStep)
		{
			Tubulin[] collidingTubulins = kinesin.tubulinDetector.GetCollidingTubulins( transform.position + moveStep, radius );
			if (collidingTubulins.Length > 0)
			{
				OnCollisionWithTubulin( collidingTubulins );
				return true;
			}
			return kinesin.CheckInternalCollision( this, moveStep );
		}

		protected abstract void OnCollisionWithTubulin (Tubulin[] collidingTubulins);

		protected abstract bool WithinLeash (Vector3 moveStep);

		public void Jitter () 
		{
			transform.position += Helpers.GetRandomVector( SampleExponentialDistribution( 0.01f ) );
		}

		float SampleExponentialDistribution (float mean)
		{
			return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / mean);
		}

		void OnTriggerEnter (Collider other)
		{
			SphereCollider sphere = other as SphereCollider;
			if (sphere != null && canMove)
			{
				Vector3 fromOther = transform.position - other.transform.position;
				Vector3 moveStep = (sphere.radius + radius - fromOther.magnitude) * fromOther.normalized;
				if (transform.parent == null || WithinLeash( moveStep ))
				{
					transform.position += moveStep;
				}
			}
		}
	}
}