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
		public Kinesin kinesin;

		protected abstract bool canMove
		{
			get;
		}

		bool isParent
		{
			get
			{
				return transform.parent == kinesin.transform;
			}
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
				if (isParent || WithinLeash( moveStep ))
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

		public void Jitter (float amount = 0.01f) 
		{
			transform.position += Helpers.GetRandomVector( SampleExponentialDistribution( amount ) );
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
				if (isParent || WithinLeash( moveStep ))
				{
					transform.position += moveStep;
				}
			}
		}

		public abstract void Reset ();
	}
}