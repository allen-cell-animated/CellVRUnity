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
		public float frequencyPerSecond;

		public EventWithKineticRate (string _name, KineticEvent _event, float _frequencyPerSecond)
		{
			name = _name;
			kineticEvent = _event;
			frequencyPerSecond = _frequencyPerSecond;
		}
	}

	public abstract class Molecule : MonoBehaviour 
	{
		public float minDistanceFromParent = 2f;
		public float maxDistanceFromParent = 6f;
		public float meanStepSize = 0.2f;
		public float meanRotation = 5f;
		public Kinesin kinesin;

		public abstract bool bound
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

		public abstract void Simulate ();

		protected abstract void DoRandomWalk ();

		protected void Rotate ()
		{
			transform.rotation *= Quaternion.Euler( Helpers.GetRandomVector( SampleExponentialDistribution( meanRotation ) ) );
		}

		protected bool Move () 
		{
			float stepSize = SampleExponentialDistribution( meanStepSize );
			Vector3 moveStep = Helpers.GetRandomVector( stepSize );
			return MoveIfValid( moveStep );
		}

		public bool MoveIfValid (Vector3 moveStep)
		{
			if (!WillCollide( moveStep ))
			{
				return MoveIfWithinLeash( moveStep );
			}
			return false;
		}

		public bool MoveIfWithinLeash (Vector3 moveStep)
		{
			if (WithinLeash( moveStep ))
			{
				transform.position += moveStep;
				return true;
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

		protected void Jitter (float amount = 0.01f) 
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
			if (WithinLeash( moveStep ))
			{
				transform.position += moveStep;
			}
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
//			Debug.Log( something.name + " " + (something.frequencyPerSecond * kinesin.nanosecondsPerStep * 1E-5f) );
			if (Random.Range( 0, 1f ) <= something.frequencyPerSecond * kinesin.nanosecondsPerStep * 1E-4f) // should be * 1E-9f
			{
				something.kineticEvent();
				return true;
			}
			return false;
		}
	}
}