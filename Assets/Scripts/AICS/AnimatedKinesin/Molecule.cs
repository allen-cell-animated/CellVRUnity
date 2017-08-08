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

		public bool MoveIfValid (Vector3 moveStep)
		{
			if (!WillCollide( moveStep ))
			{
				if (WithinLeash( moveStep ))
				{
					transform.position += moveStep;
					return true;
				}
			}
			return false;
		}

		bool WillCollide (Vector3 moveStep)
		{
			RaycastHit[] hits = body.SweepTestAll( moveStep.normalized, moveStep.magnitude, UnityEngine.QueryTriggerInteraction.Collide );
			if (hits.Length > 0)
			{
				ProcessHits( hits );
				return true;
			}
			return false;
		}

		protected abstract void ProcessHits (RaycastHit[] hits);

		protected bool WithinLeash (Vector3 moveStep)
		{
			return isParent || CheckLeash( moveStep );
		}

		protected abstract bool CheckLeash (Vector3 moveStep);

		public void Jitter (float amount = 0.01f) 
		{
			Vector3 moveStep = Helpers.GetRandomVector( SampleExponentialDistribution( amount ) );
			if (WithinLeash( moveStep ))
			{
				transform.position += moveStep;
			}
		}

		float SampleExponentialDistribution (float mean)
		{
			return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / mean);
		}

//		void OnTriggerStay (Collider other)
//		{
//			if (canMove)
//			{
//				Vector3 moveStep = 0.1f * (transform.position - other.transform.position);
//				if (WithinLeash( moveStep ))
//				{
//					transform.position += moveStep;
//				}
//			}
//		}

		public abstract void Reset ();
	}
}