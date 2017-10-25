using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.MotorProteins
{
	// A molecule object that can 
	// - randomly rotate and move while avoiding and reporting collisions
	// - do kinetic events
	public abstract class DynamicMolecule : Molecule 
	{
		public float meanStepSize = 0.2f;
		public float meanRotation = 5f;
		public bool exitCollisions = true;
		public float bindingRadius;
		public bool interactsWithOthers;

		[SerializeField] protected List<Molecule> bindingPartners = new List<Molecule>();
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
			transform.rotation *= Quaternion.Euler( Helpers.GetRandomVector( Helpers.SampleExponentialDistribution( meanRotation ) ) );
		}

		protected bool Move () 
		{
			Vector3 moveStep = Helpers.GetRandomVector( Helpers.SampleExponentialDistribution( meanStepSize ) );

			if (interactsWithOthers)
			{
				CheckForBindingPartner( moveStep );
			}

			if (!bound)
			{
				if (!WillCollideOnMove( moveStep ))
				{
					return MoveIfValid( moveStep );
				}
			}
			return false;
		}

		void CheckForBindingPartner (Vector3 moveStep)
		{
			GetCollidingMolecules( bindingPartners, this, moveStep, bindingRadius );
			if (bindingPartners.Count > 0)
			{
				InteractWithBindingPartners();
			}
		}

		bool WillCollideOnMove (Vector3 moveStep)
		{
			GetCollidingMolecules( collidingMolecules, this, moveStep );
			return (collidingMolecules.Count > 0) ? true : DoExtraCollisionChecks( moveStep );
		}

		void GetCollidingMolecules (List<Molecule> moleculeList, Molecule molecule, Vector3 moveStep, float _radius = -1f)
		{
			moleculeList.Clear();
			foreach (MoleculeDetector detector in moleculeDetectors)
			{
				moleculeList.AddRange( detector.GetCollidingMolecules( molecule, moveStep, _radius ) );
			}
		}

		protected List<Molecule> GetNearbyMolecules (MoleculeType _type)
		{
			List<Molecule> moleculeList = new List<Molecule>();
			foreach (MoleculeDetector detector in moleculeDetectors)
			{
				if (detector.moleculeType == _type)
				{
					moleculeList.AddRange( detector.nearbyMolecules );
				}
			}
			return moleculeList;
		}

		protected abstract void InteractWithBindingPartners ();

		protected abstract bool DoExtraCollisionChecks (Vector3 moveStep);

		protected virtual void Jitter (float amount = 0.01f) 
		{
			Vector3 moveStep = Helpers.GetRandomVector( Helpers.SampleExponentialDistribution( amount ) );
			MoveIfValid( moveStep );
		}

		public bool MoveIfValid (Vector3 moveStep)
		{
			if (IsValidMove( moveStep ))
			{
				IncrementPosition( moveStep );
				return true;
			}
			return false;
		}

		protected abstract bool IsValidMove (Vector3 moveStep);

//		void OnTriggerStay (Collider other)
//		{
//			Debug.Log( name );
//			if (!bound && exitCollisions && !MolecularEnvironment.Instance.pause)
//			{
//				ExitCollision( other.transform.position );
//			}
//		}
//
//		void ExitCollision (Vector3 otherPosition)
//		{
//			Vector3 moveStep = 0.1f * (transform.position - otherPosition);
//			MoveIfValid( moveStep );
//		}

		protected void DoInRandomOrder (Kinetic[] kinetics)
		{
			if (kinetics.Length > 0)
			{
				kinetics.Shuffle();
				for (int i = 0; i < kinetics.Length; i++)
				{
					if (DoEventAtKineticRate( kinetics[i] ))
					{
						return;
					}
				}
			}
		}

		protected bool DoEventAtKineticRate (Kinetic kinetic)
		{
			kinetic.attempts++;
			if (kinetic.ShouldHappen())
			{
				if (kinetic.kineticEvent( kinetic ))
				{
					kinetic.events++;
					return true;
				}
			}
			return false;
		}
	}
}