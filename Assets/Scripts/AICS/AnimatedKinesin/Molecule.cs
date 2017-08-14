using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.AnimatedKinesin
{
	public delegate void KineticEvent ();

	public class EventWithKineticRate
	{
		public string name;
		public KineticEvent kineticEvent;
		public KineticRate frequencyPerSecond;

		public EventWithKineticRate (string _name, KineticEvent _event, KineticRate _frequencyPerSecond)
		{
			name = _name;
			kineticEvent = _event;
			frequencyPerSecond = _frequencyPerSecond;
		}
	}

	// A basic molecule object that can 
	// - randomly rotate and move while avoiding and reporting collisions
	// - do kinetic events
	public abstract class Molecule : MonoBehaviour 
	{
		public float radius;
		public float meanStepSize = 0.2f;
		public float meanRotation = 5f;

		public abstract bool bound
		{
			get;
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

		public abstract void Simulate ();

		public abstract void DoRandomWalk ();

		protected void Rotate ()
		{
			transform.rotation *= Quaternion.Euler( Helpers.GetRandomVector( SampleExponentialDistribution( meanRotation ) ) );
		}

		protected bool Move () 
		{
			Vector3 moveStep = Helpers.GetRandomVector( SampleExponentialDistribution( meanStepSize ) );
			if (!WillCollide( moveStep ))
			{
				return MoveIfValid( moveStep );
			}
			return false;
		}

		bool WillCollide (Vector3 moveStep)
		{
			if (MolecularEnvironment.Instance.collisionDetectionMethod == CollisionDetectionMethod.Sweeptest)
			{
				return CheckCollisionsSweeptest( moveStep );
			}
			else if (MolecularEnvironment.Instance.collisionDetectionMethod == CollisionDetectionMethod.Spheres)
			{
				return CheckCollisionsSpheres( moveStep );
			}
			return false;
		}

		bool CheckCollisionsSweeptest (Vector3 moveStep)
		{
			RaycastHit[] hits = body.SweepTestAll( moveStep.normalized, moveStep.magnitude, UnityEngine.QueryTriggerInteraction.Collide );
			if (hits.Length > 0)
			{
				ProcessHits( hits );
				return true;
			}
			return false;
		}

		bool CheckCollisionsSpheres (Vector3 moveStep)
		{
			// TODO
			return false;
		}

		protected abstract void ProcessHits (RaycastHit[] hits);

		protected virtual void Jitter (float amount = 0.01f) 
		{
			Vector3 moveStep = Helpers.GetRandomVector( SampleExponentialDistribution( amount ) );
			MoveIfValid( moveStep );
		}

		public bool MoveIfValid (Vector3 moveStep)
		{
			if (IsValidMove( moveStep ))
			{
				transform.position += moveStep;
				return true;
			}
			return false;
		}

		protected abstract bool IsValidMove (Vector3 moveStep);

		float SampleExponentialDistribution (float mean)
		{
			return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / mean);
		}

		void OnTriggerStay (Collider other)
		{
			if (!bound)
			{
				ExitCollision( other.transform.position );
			}
		}

		void ExitCollision (Vector3 otherPosition)
		{
			Vector3 moveStep = 0.1f * (transform.position - otherPosition);
			MoveIfValid( moveStep );
		}

		public abstract void Reset ();

		protected void DoInRandomOrder (EventWithKineticRate[] things)
		{
			if (things.Length > 0)
			{
				things.Shuffle();
				for (int i = 0; i < things.Length; i++)
				{
					if (DoSomethingAtKineticRate( things[i] ))
					{
						return;
					}
				}
			}
		}

		protected bool DoSomethingAtKineticRate (EventWithKineticRate something)
		{
			if (Random.Range( 0, 1f ) <= something.frequencyPerSecond.rate * MolecularEnvironment.Instance.nanosecondsPerStep * 1E-9f)
			{
				something.kineticEvent();
				return true;
			}
			return false;
		}
	}
}