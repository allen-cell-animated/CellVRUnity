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
		protected float startRotatingNanoseconds = -100f;
		protected float rotateDuration = 1f;
		protected float rotateSpeed = 0.0005f;
		float totalRotationAngle;
		bool exiting = false;
		public bool binding = false;
		public float moveRandomness = 0.9f;
		public float rotateRandomness = 1f;

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

        protected void Animate (bool forceMove = false) // Called every frame, forceMove is true if binding to tubulin
		{
			if (rotating)
			{
				AnimateRotation( (MolecularEnvironment.Instance.nanosecondsSinceStart - startRotatingNanoseconds) / rotateDuration );
			}
			if (moving)
			{
				AnimateMove( (MolecularEnvironment.Instance.nanosecondsSinceStart - startMovingNanoseconds) / moveDuration, forceMove );
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
		}

		void StartRotation (Quaternion goalRotation, bool random)
		{
			startMoveRotation = transform.rotation;
			goalMoveRotation = (random ? transform.rotation * Quaternion.Euler( Helpers.GetRandomVector( Helpers.SampleExponentialDistribution( meanRotation ) ) ) : goalRotation);
			totalRotationAngle = Mathf.Abs( Quaternion.Angle( startMoveRotation, goalMoveRotation ) );
			rotateDuration = totalRotationAngle / rotateSpeed;
			startRotatingNanoseconds = MolecularEnvironment.Instance.nanosecondsSinceStart;
			if (rotateDuration <= MolecularEnvironment.Instance.nanosecondsPerStep)
			{
				AnimateRotation( 1f );
			}
			else
			{
				rotating = true;
			}
		}

		void AnimateRotation (float t)
		{
			if (rotating && t >= 1f)
			{
				rotating = false;
			}
			transform.rotation = Quaternion.Slerp( startMoveRotation, goalMoveRotation, Mathf.Clamp( t, 0, 1f ) ) 
				* Quaternion.Euler( Helpers.GetRandomVector( rotateRandomness * totalRotationAngle ) );
		}

		protected bool MoveRandomly (bool retry = false) 
		{
			float t = (MolecularEnvironment.Instance.nanosecondsSinceStart - startMovingNanoseconds) / moveDuration;
			if (MolecularEnvironment.Instance.nanosecondsSinceStart == 0 || t >= 1f || retry)
			{
				Vector3 moveStep = StartMove( Vector3.zero, true );
				if (moveStep != Vector3.zero)
				{
					return StartMove( -moveStep, false ) == Vector3.zero;
				}
				return true;
			}
			else
			{
				return AnimateMove( t );
			}
		}

        protected void MoveTo (Vector3 goalPosition, bool forceMove = false) // called on exit collision and move to tubulin
		{
			StartMove( goalPosition, false, forceMove ); 
		}

		Vector3 StartMove (Vector3 goalPosition, bool random, bool forceMove = false)
		{
			startMovePosition = transform.position;
			Vector3 moveStep = Helpers.GetRandomVector( random ? Helpers.SampleExponentialDistribution( meanStepSize ) : (goalPosition - transform.position).magnitude );
			goalMovePosition = (random ? transform.position + moveStep : goalPosition);
			moveDuration = Vector3.Distance( startMovePosition, goalMovePosition ) / moveSpeed;
			startMovingNanoseconds = MolecularEnvironment.Instance.nanosecondsSinceStart;
			bool moved = true;
			if (moveDuration <= MolecularEnvironment.Instance.nanosecondsPerStep)
			{
				moving = true;
				moved = AnimateMove( 1f, forceMove );
			}
			else 
			{
				moving = true;
			}
			return (!moved ? moveStep : Vector3.zero);
		}

		bool AnimateMove (float t, bool forceMove = false) 
		{
			bool success = false;
			Vector3 moveStep = Vector3.Lerp( startMovePosition, goalMovePosition, Mathf.Clamp( t, 0, 1f ) ) - transform.position;
			moveStep += Helpers.GetRandomVector( moveRandomness * moveStep.magnitude );

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
						success = true;
					}
					else
					{
						success = MoveIfValid( moveStep );
					}
				}
			}

			if (moving && t >= 1f)
			{
				moving = false;
				OnFinishMove();
			}
			return success;
		}

		protected virtual void OnFinishMove () { }

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
			if (!bound && !binding && exitCollisions && !MolecularEnvironment.Instance.pause)
			{
				ExitCollision( other.transform.position );
			}
            CustomOnTriggerStay(other);
        }

        protected virtual void CustomOnTriggerStay (Collider other) { }

		void ExitCollision (Vector3 otherPosition)
		{
			if (!exiting)
			{
                MoveTo( GetExitVector( otherPosition ), true );
				exiting = true;
				Invoke( "FinishExit", 1f );
			}
		}

        protected virtual Vector3 GetExitVector (Vector3 otherPosition)
        {
            return transform.position + 0.5f * (transform.position - otherPosition);
        }

		void FinishExit ()
		{
			exiting = false;
		}

		protected bool DoInRandomOrder (Kinetic[] kinetics)
		{
			if (kinetics.Length > 0)
			{
				kinetics.Shuffle();
				for (int i = 0; i < kinetics.Length; i++)
				{
					if (DoEventAtKineticRate( kinetics[i] ))
					{
						return true;
					}
				}
			}
			return false;
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