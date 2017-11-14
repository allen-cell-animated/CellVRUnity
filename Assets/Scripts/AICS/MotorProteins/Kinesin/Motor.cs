using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MT;
using System.IO;
using AICS.UI;
using AICS.PhysicsKinesin;

namespace AICS.MotorProteins.Kinesin
{
	public enum MotorState 
	{
		MtK, // bound to MT with no nucleotide
		MtKT, // bound to MT with ATP bound
		MtKDP, // bound to MT with ADP and Pi bound
		MtKD, // bound to MT with ADP bound
		KDP, // free moving with ADP and Pi bound
		KD // free moving with ADP bound
	}

	public class Motor : ComponentMolecule 
	{
		public MotorState state = MotorState.KD;
		public float bindingRotationTolerance = 30f;
//		public GameObject ATP;
//		public GameObject ADP;
//		public GameObject Pi;
		public bool needToSwitchToStrong;
		public StateIndicator stateIndicatorUI;
		public Direction forwardDirection;
		public Direction upDirection;
		public Kinetics kinetics;
		public Transform atpBindingSite;

		Vector3 bindingPosition = new Vector3( 0, 4.53f, 0 );
		Vector3 bindingRotation = new Vector3( 0, 0, 0 );
		public Tubulin tubulin;
		float lastReleaseTime = -1f;
		float lastSetToBindingPositionTime = -1f;
		public Color tubulinFlashColor;
		public Nucleotide boundNucleotide;
		MotorState finalBindState;
		public TubulinGraph tubulinGraph;
		bool justChangedState = false;

		Kinesin kinesin
		{
			get
			{
				return assembly as Kinesin;
			}
		}

		Motor _otherMotor;
		Motor otherMotor
		{
			get
			{
				if (_otherMotor == null)
				{
					_otherMotor = kinesin.motors.Find( m => m != this );
				}
				return _otherMotor;
			}
		}

		Collider _collider;
		Collider theCollider
		{
			get
			{
				if (_collider == null)
				{
					_collider = GetComponent<Collider>();
				}
				return _collider;
			}
		}

		void SetState (MotorState newState)
		{
			state = newState;
//			stateIndicatorUI.GoToState( (int)state );
		}

		protected override void OnAwake () { }

		public static Kinetic GetNextEvent (MotorState startState)
		{
			return new Kinetic( new KineticRate( "", 0, 0, 1f ) );
		}

		public static float GetEventNanoseconds (Kinetic transition)
		{
			return 0;
		}

		void Start ()
		{
			kinetics = new Kinetics( kinesin.kineticRates );
			tubulinGraph = new TubulinGraph( forwardDirection, upDirection );
		}

		// --------------------------------------------------------------------------------------------------- State

		public override bool bound
		{
			get
			{
				return (int)state <= (int)MotorState.MtKD;
			}
		}

		bool stateIsStrong
		{
			get
			{
				return state == MotorState.MtK || state == MotorState.MtKT;
			}
		}

		Dictionary<MotorState,Kinetic[]> _eventsForState;
		Dictionary<MotorState,Kinetic[]> eventsForState
		{
			get 
			{
				if (_eventsForState == null)
				{
					SetEventsForStates();
				}
				return _eventsForState;
			}
		}

		void SetEventsForStates ()
		{
			_eventsForState = new Dictionary<MotorState,Kinetic[]>();

			Kinetic[] events = new Kinetic[1];
			events[0] = kinetics.GetKinetic( (int)MotorState.MtK, (int)MotorState.MtKT );
			events[0].kineticEvent = BindATP;
			_eventsForState.Add( MotorState.MtK, events );

			events = new Kinetic[2];
			events[0] = kinetics.GetKinetic( (int)MotorState.MtKT, (int)MotorState.MtK );
			events[0].kineticEvent = ReleaseATP;
			events[1] = kinetics.GetKinetic( (int)MotorState.MtKT, (int)MotorState.MtKDP );
			events[1].kineticEvent = HydrolyzeATP;
			_eventsForState.Add( MotorState.MtKT, events );

			events = new Kinetic[2];
			events[0] = kinetics.GetKinetic( (int)MotorState.MtKDP, (int)MotorState.MtKD );
			events[0].kineticEvent = ReleasePhosphate;
			events[1] = kinetics.GetKinetic( (int)MotorState.MtKDP, (int)MotorState.KDP );
			events[1].kineticEvent = ReleaseTubulin;
			_eventsForState.Add( MotorState.MtKDP, events );

			events = new Kinetic[2];
			events[0] = kinetics.GetKinetic( (int)MotorState.MtKD, (int)MotorState.MtK );
			events[0].kineticEvent = ReleaseADP;
			events[1] = kinetics.GetKinetic( (int)MotorState.MtKD, (int)MotorState.KD );
			events[1].kineticEvent = ReleaseTubulin;
			_eventsForState.Add( MotorState.MtKD, events );

			events = new Kinetic[2];
			events[0] = kinetics.GetKinetic( (int)MotorState.KDP, (int)MotorState.MtKDP );
			events[0].kineticEvent = BindTubulin;
			events[1] = kinetics.GetKinetic( (int)MotorState.KDP, (int)MotorState.KD );
			events[1].kineticEvent = ReleasePhosphate;
			_eventsForState.Add( MotorState.KDP, events );

			events = new Kinetic[1];
			events[0] = kinetics.GetKinetic( (int)MotorState.KD, (int)MotorState.MtKD );
			events[0].kineticEvent = BindTubulin;
			_eventsForState.Add( MotorState.KD, events );
		}

		public override void DoCustomSimulation ()
		{
			if (resetFrames == 0)
			{
				if (!binding)
				{
					DoRandomWalk();

					if (needToSwitchToStrong)
					{
						TryToSwitchToStrong();
					}

					if (Random.value <= (justChangedState ? 0.2f : 1f))
					{
						justChangedState = DoInRandomOrder( eventsForState[state] );
					}
					else
					{
						justChangedState = false;
					}
				}
					
				Animate( binding );
				kinetics.CalculateObservedRates();
			}
			else
			{
				body.isKinematic = theCollider.isTrigger = binding = moving = rotating = false;
			}
		}

		void TryToSwitchToStrong ()
		{
			if (bound)
			{
				if (state == MotorState.MtKDP)
				{
					Kinetic k = kinetics.GetKinetic( (int)MotorState.MtKDP, (int)MotorState.MtKD );
					if (!k.observedRateTooHigh)
					{
						k.attempts++;
						ReleasePhosphate( k );
					}
				}
				else if (state == MotorState.MtKD)
				{
					Kinetic k = kinetics.GetKinetic( (int)MotorState.MtKD, (int)MotorState.MtK );
					if (!k.observedRateTooHigh)
					{
						k.attempts++;
						ReleaseADP( k );
						needToSwitchToStrong = false;
					}
				}
			}
			else 
			{
				if (state == MotorState.KDP || state == MotorState.KD)
				{
					Kinetic k = (state == MotorState.KDP) ? 
						kinetics.GetKinetic( (int)MotorState.KDP, (int)MotorState.MtKDP ) : kinetics.GetKinetic( (int)MotorState.KD, (int)MotorState.MtKD );
					if (!k.observedRateTooHigh)
					{
						k.attempts++;
						BindTubulin( k );
					}
				}
			}
		}

		bool BindATP (Kinetic kinetic)
		{
			if (FindAndBindATP())
			{
				if (logEvents) { Debug.Log( name + " bind ATP" ); }
				SetState( (MotorState)kinetic.finalStateIndex );
				kinesin.hips.StartSnap( this );
				return true;
			}
			return false;
		}

		bool ReleaseATP (Kinetic kinetic)
		{
			if (boundNucleotide != null)
			{
				if (logEvents) { Debug.Log( name + " release ATP" ); }

				SetState( (MotorState)kinetic.finalStateIndex );
				DoNucleotideRelease();
				return true;
			}
			return false;
		}

		bool HydrolyzeATP (Kinetic kinetic)
		{
			if (logEvents) { Debug.Log( name + " hydrolyze" ); }

			SetState( (MotorState)kinetic.finalStateIndex );
			if (boundNucleotide != null)
			{
				boundNucleotide.Hydrolyze();
			}
			otherMotor.SwitchToStrong();
			return true;
		}

		void SwitchToStrong ()
		{
			needToSwitchToStrong = !stateIsStrong;
		}

		bool ReleasePhosphate (Kinetic kinetic)
		{
			if (logEvents) { Debug.Log( name + " release Pi" ); }

			SetState( (MotorState)kinetic.finalStateIndex );
			if (boundNucleotide != null)
			{
				boundNucleotide.ReleasePi();
			}
			return true;
		}

		bool ReleaseADP (Kinetic kinetic)
		{
			if (!otherMotor.stateIsStrong)
			{
				if (logEvents) { Debug.Log( name + " release ADP" ); }

				SetState( (MotorState)kinetic.finalStateIndex );
				DoNucleotideRelease();
				return true;
			}
			return false;
		}

		// --------------------------------------------------------------------------------------------------- Nucleotide binding

		bool FindAndBindATP ()
		{
			boundNucleotide = kinesin.atpGenerator.GetClosestATPToPoint( atpBindingSite.position );
			if (boundNucleotide != null)
			{
				DoNucleotideBind();
				return true;
			}
			return false;
		}

		void DoNucleotideBind ()
		{
			boundNucleotide.transform.SetParent( atpBindingSite );
			boundNucleotide.transform.localPosition = Vector3.zero;
			boundNucleotide.transform.localRotation = Quaternion.identity;
			boundNucleotide.isBusy = true;
			boundNucleotide.GetComponent<Rigidbody>().isKinematic = true;
//			Rigidbody body = boundNucleotide.GetComponent<Rigidbody>();
//			if (body != null) 
//			{
//				Destroy( body );
//			}
		}

		void DoNucleotideRelease ()
		{
			if (boundNucleotide != null)
			{
//				if (!boundNucleotide.GetComponent<Rigidbody>())
//				{
//					Rigidbody body = boundNucleotide.gameObject.AddComponent<Rigidbody>();
//					body.useGravity = false;
//					body.mass = nucleotide.mass;
//					body.drag = 5f;
//				}
				boundNucleotide.GetComponent<Rigidbody>().isKinematic = false;
				boundNucleotide.transform.SetParent( boundNucleotide.parent );
				boundNucleotide.isBusy = false;
				boundNucleotide = null;
			}
		}

		// --------------------------------------------------------------------------------------------------- Random walk

		public override void DoRandomWalk ()
		{
			if (!bound)
			{
				RotateRandomly();

				int i = 0;
				bool retry = false;
				bool success = false;
				while (!bound && !success && i < MolecularEnvironment.Instance.maxIterationsPerStep)
				{
					success = MoveRandomly( retry );
					retry = true;
					i++;
				}

				if (!success)
				{
					Jitter( 0.1f );
				}
			}
			else 
			{
				Jitter( 0.001f );
				if (Time.time - lastSetToBindingPositionTime >= 1f)
				{
					SetPosition( tubulin.transform.TransformPoint( bindingPosition ) );
					lastSetToBindingPositionTime = Time.time;
				}
			}
		}

		protected override bool CheckLeash (Vector3 moveStep)
		{
			float d = Vector3.Distance( transform.parent.position, transform.position + moveStep );
			return d >= minDistanceFromParent && d <= maxDistanceFromParent;
		}

		// --------------------------------------------------------------------------------------------------- Tubulin binding/release

		bool BindTubulin (Kinetic kinetic)
		{
			if (!bound && !binding && !otherMotor.binding && (Time.time - lastReleaseTime > 0.5f || needToSwitchToStrong))
			{
				Tubulin t = FindTubulin();
				if (t != null)
				{
					StartTubulinBind( t, kinetic );
					return true;
				}
			}
			return false;
		}

		Tubulin FindTubulin ()
		{
			if (otherMotor.tubulinGraph.empty)
			{
				return GetClosestValidTubulin();
			}
			else
			{
				return GetTubulinAroundOtherMotor();
			}
		}

		Tubulin GetClosestValidTubulin ()
		{
			List<Tubulin> tubulins = GetTubulins();

			float d, minDistance = Mathf.Infinity;
			Tubulin closest = null;
			foreach (Tubulin t in tubulins)
			{
				if (TubulinIsValid( t ) && !t.hasMotorBound)
				{
					d = Vector3.Distance( t.transform.position, transform.position );
					if (d < minDistance)
					{
						minDistance = d;
						closest = t;
					}
				}
			}
			return closest;
		}

		public List<TubulinAngle> molecules = new List<TubulinAngle>();

		Tubulin GetTubulinAroundOtherMotor ()
		{
			molecules.Clear();
			foreach (TubulinAngle ma in otherMotor.tubulinGraph.molecules)
			{
				Tubulin t = ma.molecule as Tubulin;
				if (t != null && TubulinIsValid( t ))
				{
					molecules.Add( ma );
//					t.Flash( tubulinFlashColor );
				}
			}

			if (molecules.Count > 0)
			{
//				molecules.Sort();
				return molecules[0].molecule as Tubulin;
			}
			return null;
		}

		List<Tubulin> GetTubulins ()
		{
			Tubulin t;
			List<Tubulin> tubulins = new List<Tubulin>();
			List<Molecule> nearbyMolecules = GetNearbyMolecules( MoleculeType.Tubulin );
			foreach (Molecule m in nearbyMolecules)
			{
				t = m as Tubulin;
				if (t != null && t != tubulin && t.tubulinType == 1)
				{
					tubulins.Add( t );
				}
			}
			return tubulins;
		}

		bool TubulinIsValid (Tubulin _tubulin)
		{
			Vector3 _bindingPosition = _tubulin.transform.TransformPoint( bindingPosition );
			float hipsDistance = Vector3.Distance( _bindingPosition, kinesin.hips.transform.position );
			return _tubulin.tubulinType == 1 && hipsDistance <= maxDistanceFromParent; // && CloseToBindingOrientation( t ) && !_tubulin.hasMotorBound 
		}

		bool CloseToBindingOrientation (Tubulin _tubulin)
		{
			Vector3 localRotation = (Quaternion.Inverse( _tubulin.transform.rotation ) * transform.rotation).eulerAngles;
			return Helpers.AngleIsWithinTolerance( localRotation.x, bindingRotation.x, bindingRotationTolerance )
				&& Helpers.AngleIsWithinTolerance( localRotation.y, bindingRotation.y, bindingRotationTolerance )
				&& Helpers.AngleIsWithinTolerance( localRotation.z, bindingRotation.z, bindingRotationTolerance );
		}

		int GetRandomIndex (int n)
		{
			return Mathf.CeilToInt( Random.Range( Mathf.Epsilon, 1f ) * n ) - 1;
		}

		int GetExponentialRandomIndex (int n)
		{
			float i = Mathf.Clamp( -Mathf.Log10( Random.Range( Mathf.Epsilon, 1f ) ) / 2f, 0, 1f );
			return Mathf.CeilToInt( i * n ) - 1;
		}

		void StartTubulinBind (Tubulin _tubulin, Kinetic kinetic)
		{
			if (logEvents) { Debug.Log( name + " start BIND" ); }

			binding = true;
			finalBindState = (MotorState)kinetic.finalStateIndex;
			tubulin = _tubulin;
			kinesin.lastTubulin = tubulin;
			tubulin.hasMotorBound = true;

			kinesin.SetParentSchemeOnComponentBind( this as ComponentMolecule );
			MoveTo( tubulin.transform.TransformPoint( bindingPosition ) );
			RotateTo( tubulin.transform.rotation * Quaternion.Euler( bindingRotation ) );
		}

		// we can get collision events while the motor is still binding and its rigidbody is not yet kinematic
		void OnCollisionEnter (Collision collision)
		{
			if (collision.collider.tag == "Player")
			{
				if (binding)
				{
					Kinetic k = kinetics.GetKinetic( (int)state, (int)state - 2 );
					k.events--;
					CancelTubulinBind();
				}
			}
		}

		// once the motor is bound and rigidbody is kinematic, only get trigger events
		void OnTriggerEnter (Collider other)
		{
			if (other.tag == "Player")
			{
				if (bound && !stateIsStrong)
				{
					Kinetic k = kinetics.GetKinetic( (int)state, (int)state + 2 );
					k.attempts++;
					if (ReleaseTubulin( k ))
					{
						k.events++;
					}
				}
			}
		}

		public void CancelTubulinBind ()
		{
			if (logEvents) { Debug.Log( name + " cancel BIND" ); }

			if (tubulin != null)
			{
				tubulin.hasMotorBound = false;
			}
			kinesin.SetParentSchemeOnComponentRelease( this as ComponentMolecule );
			lastReleaseTime = Time.time;
			binding = false;
		}

		protected override void OnFinishMove () 
		{
			if (binding) 
			{
				FinishTubulinBind();
			}
		}

		void FinishTubulinBind ()
		{
			if (logEvents) { Debug.Log( name + " finish BIND" ); }

			SetState( finalBindState );
			SetPosition( tubulin.transform.TransformPoint( bindingPosition ) );
			transform.rotation = tubulin.transform.rotation * Quaternion.Euler( bindingRotation );
			lastSetToBindingPositionTime = Time.time;
			tubulinGraph.SetMolecules( GetTubulins(), transform );
			body.isKinematic = true;
			theCollider.isTrigger = true;
			binding = false;
		}

		bool ReleaseTubulin (Kinetic kinetic)
		{
			if (otherMotor.bound)
			{
				if (logEvents) { Debug.Log( name + " RELEASE" ); }

				SetState( (MotorState)kinetic.finalStateIndex );
				kinesin.SetParentSchemeOnComponentRelease( this as ComponentMolecule );
				lastReleaseTime = Time.time;
				if (tubulin != null)
				{
					tubulin.hasMotorBound = false;
					Vector3 fromTubulin = (transform.position - tubulin.transform.position).normalized;
					MoveIfValid( fromTubulin );
				}
				kinesin.hips.SetFree( this );
				tubulinGraph.Clear();
				body.isKinematic = false;
				theCollider.isTrigger = false;
				return true;
			}
			return false;
		}

		protected override void InteractWithBindingPartners () { }

		// --------------------------------------------------------------------------------------------------- Reset

		public override void DoCustomReset ()
		{
			if (logEvents) { Debug.Log( name + " reset" ); }

			SetState( MotorState.KD );
			needToSwitchToStrong = false;
			tubulin = null;
			lastReleaseTime = -1f;
//			ATP.SetActive( false );
//			ADP.SetActive( true );
//			Pi.SetActive( false );
			kinetics.Reset();
			tubulinGraph.Clear();
			body.isKinematic = false;
			theCollider.isTrigger = false;
			binding = moving = rotating = false;
		}
	}
}