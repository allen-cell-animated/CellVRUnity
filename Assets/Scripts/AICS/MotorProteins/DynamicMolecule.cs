using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.MotorProteins
{
	public delegate void KineticEvent ();

	public class EventWithKineticRate
	{
		public string name;
		public KineticEvent kineticEvent;
		public Kinetic kinetic;

		public EventWithKineticRate (string _name, KineticEvent _event, Kinetic _kinetic)
		{
			name = _name;
			kineticEvent = _event;
			kinetic = _kinetic;
		}
	}

	// A molecule object that can 
	// - randomly rotate and move while avoiding and reporting collisions
	// - do kinetic events
	public abstract class DynamicMolecule : Molecule 
	{
		public float meanStepSize = 0.2f;
		public float meanRotation = 5f;
		public bool exitCollisions = true;
		public bool interactsWithOtherMolecules;
		public float bindingRadius;

		protected List<Molecule> bindingPartners = new List<Molecule>();
		protected List<Molecule> collidingMolecules = new List<Molecule>();

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

		protected void Rotate ()
		{
			transform.rotation *= Quaternion.Euler( Helpers.GetRandomVector( SampleExponentialDistribution( meanRotation ) ) );
		}

		protected bool Move () 
		{
			Vector3 moveStep = Helpers.GetRandomVector( SampleExponentialDistribution( meanStepSize ) );
//			CheckForBindingPartner( moveStep );
//			if (!bound)
//			{
				if (!WillCollideOnMove( moveStep ))
				{
					return MoveIfValid( moveStep );
				}
//			}
			return false;
		}

		bool CheckForBindingPartner (Vector3 moveStep)
		{
			GetCollidingMolecules( bindingPartners, this, moveStep, bindingRadius );
			if (bindingPartners.Count > 0)
			{
				if (interactsWithOtherMolecules)
				{
					// interact with binding partners here instead of in WillCollideOnMove
					InteractWithCollidingMolecules();
				}
				return true;
			}
			return false;
		}

		bool WillCollideOnMove (Vector3 moveStep)
		{
			GetCollidingMolecules( collidingMolecules, this, moveStep );
			if (collidingMolecules.Count > 0)
			{
				if (interactsWithOtherMolecules)
				{
					InteractWithCollidingMolecules();
				}
				return true;
			}
			return DoExtraCollisionChecks( moveStep );
		}

		void GetCollidingMolecules (List<Molecule> moleculeList, Molecule molecule, Vector3 moveStep, float _radius = -1f)
		{
			moleculeList.Clear();
			foreach (MoleculeDetector detector in moleculeDetectors)
			{
				moleculeList.AddRange( detector.GetCollidingMolecules( molecule, moveStep, _radius ) );
			}
		}

		protected abstract void InteractWithCollidingMolecules ();

		protected abstract bool DoExtraCollisionChecks (Vector3 moveStep);

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

		protected float SampleExponentialDistribution (float mean)
		{
			return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / mean);
		}

		void OnTriggerStay (Collider other)
		{
			if (!bound && exitCollisions && !MolecularEnvironment.Instance.pause)
			{
				ExitCollision( other.transform.position );
			}
		}

		void ExitCollision (Vector3 otherPosition)
		{
			Vector3 moveStep = 0.1f * (transform.position - otherPosition);
			MoveIfValid( moveStep );
		}

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
			something.kinetic.attempts++;
			if (something.kinetic.ShouldHappen( MolecularEnvironment.Instance.nanosecondsPerStep, stepsSinceStart ))
			{
				something.kineticEvent();
				return true;
			}
			return false;
		}
	}
}