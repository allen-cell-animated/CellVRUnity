using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

		Vector3 startMovePosition;
		Vector3 goalMovePosition;
		Quaternion startMoveRotation;
		Quaternion goalMoveRotation;
		public bool moving = false;
		float startMovingNanoseconds;
		float moveDuration;
		float moveSpeed = 0.00005f;
		public bool rotating = false;
		float startRotatingNanoseconds;
		float rotateDuration;
		float rotateSpeed = 0.0005f;
		bool exiting = false;

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

		protected void Animate (bool forceMove = false)
		{
			if (moving)
			{
				AnimateMove( (MolecularEnvironment.Instance.nanosecondsSinceStart - startMovingNanoseconds) / moveDuration, forceMove );
			}
			if (rotating)
			{
				AnimateRotation( (MolecularEnvironment.Instance.nanosecondsSinceStart - startRotatingNanoseconds) / rotateDuration );
			}
		}

		protected void RotateRandomly ()
		{
			float t = (MolecularEnvironment.Instance.nanosecondsSinceStart - startRotatingNanoseconds) / rotateDuration;
			if (MolecularEnvironment.Instance.nanosecondsSinceStart == 0 || t >= 1f)
			{
				StartRotation( Quaternion.identity, true );
			}
			else 
			{
				AnimateRotation( t );
			}
		}

		protected void RotateTo (Quaternion goalRotation)
		{
			StartRotation( goalRotation, false );
			rotating = true;
		}

		void StartRotation (Quaternion goalRotation, bool random)
		{
			startMoveRotation = transform.rotation;
			goalMoveRotation = (random ? transform.rotation * Quaternion.Euler( Helpers.GetRandomVector( Helpers.SampleExponentialDistribution( meanRotation ) ) ) : goalRotation);
			rotateDuration = Mathf.Abs( Quaternion.Angle( startMoveRotation, goalMoveRotation ) ) / rotateSpeed;
			if (rotateDuration <= MolecularEnvironment.Instance.nanosecondsPerStep)
			{
				AnimateRotation( 1f );
			}
			else
			{
				startRotatingNanoseconds = MolecularEnvironment.Instance.nanosecondsSinceStart;
			}
		}

		void AnimateRotation (float t)
		{
			if (rotating && t >= 1f)
			{
				rotating = false;
			}
			transform.rotation = Quaternion.Slerp( startMoveRotation, goalMoveRotation, Mathf.Min( 1f, t ) );
		}

		protected bool MoveRandomly (bool retry = false) 
		{
			float t = (MolecularEnvironment.Instance.nanosecondsSinceStart - startMovingNanoseconds) / moveDuration;
			if (MolecularEnvironment.Instance.nanosecondsSinceStart == 0 || t >= 1f || retry)
			{
				return StartMove( Vector3.zero, true );
			}
			else
			{
				return AnimateMove( t );
			}
		}

		protected void MoveTo (Vector3 goalPosition)
		{
			StartMove( goalPosition, false );
			moving = true;
		}

		bool StartMove (Vector3 goalPosition, bool random)
		{
			startMovePosition = transform.position;
			goalMovePosition = (random ? transform.position + Helpers.GetRandomVector( Helpers.SampleExponentialDistribution( meanStepSize ) ) : goalPosition);
			moveDuration = Vector3.Distance( startMovePosition, goalMovePosition ) / moveSpeed;
			if (moveDuration <= MolecularEnvironment.Instance.nanosecondsPerStep)
			{
				return AnimateMove( 1f );
			}
			startMovingNanoseconds = MolecularEnvironment.Instance.nanosecondsSinceStart;
			return true;
		}

		bool AnimateMove (float t, bool forceMove = false) 
		{
			Vector3 moveStep = Vector3.Lerp( startMovePosition, goalMovePosition, Mathf.Min( 1f, t ) ) - transform.position;
			if (moving && t >= 1f)
			{
				moving = false;
			}

			if (interactsWithOthers)
			{
				CheckForBindingPartner( moveStep );
			}

			if (!bound || forceMove)
			{
				if (!WillCollideOnMove( moveStep ) || forceMove)
				{
					if (forceMove)
					{
						IncrementPosition( moveStep );
						return true;
					}
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

		void OnTriggerStay (Collider other)
		{
			if (!bound && exitCollisions && !MolecularEnvironment.Instance.pause && other.GetComponent<PhysicsKinesin.Nucleotide>() == null)
			{
				ExitCollision( other.transform.position );
			}
		}

		void ExitCollision (Vector3 otherPosition)
		{
			if (!exiting)
			{
				MoveTo( transform.position + 0.1f * (transform.position - otherPosition) );
				exiting = true;
				Invoke( "FinishExit", 1f );
			}
		}

		void FinishExit ()
		{
			exiting = false;
		}

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