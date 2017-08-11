using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.AnimatedKinesin
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

	public class Motor : Molecule 
	{
		public bool logEvents;
		public MotorState state = MotorState.KD;
		public float bindingRotationTolerance = 30f;
		public GameObject ATP;
		public GameObject ADP;
		public GameObject Pi;
		public GameObject chargeForceFields;
		public bool needToSwitchToStrong;

		Vector3 bindingPosition = new Vector3( 0, 4.53f, 0 );
		Vector3 bindingRotation = new Vector3( 0, 0, 0 );
		Tubulin tubulin;
		float lastReleaseTime = -1f;
		float lastSetToBindingPositionTime = -1f;

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

		Dictionary<MotorState,EventWithKineticRate[]> _actionsForState;
		Dictionary<MotorState,EventWithKineticRate[]> actionsForState
		{
			get 
			{
				if (_actionsForState == null)
				{
					SetActionsForStates();
				}
				return _actionsForState;
			}
		}

		void SetActionsForStates ()
		{
			_actionsForState = new Dictionary<MotorState, EventWithKineticRate[]>();

			EventWithKineticRate[] actions = new EventWithKineticRate[1];
			actions[0] = new EventWithKineticRate( "BindATP", BindATP, kinesin.kineticRates.GetRate( "A" ) );
			_actionsForState.Add( MotorState.MtK, actions );

			actions = new EventWithKineticRate[2];
			actions[0] = new EventWithKineticRate( "ReleaseATP", ReleaseATP, kinesin.kineticRates.GetRate( "B" ) );
			actions[1] = new EventWithKineticRate( "HydrolyzeATP", HydrolyzeATP, kinesin.kineticRates.GetRate( "C" ) );
			_actionsForState.Add( MotorState.MtKT, actions );

			actions = new EventWithKineticRate[2];
			actions[0] = new EventWithKineticRate( "ReleasePhosphate", ReleasePhosphate, kinesin.kineticRates.GetRate( "D" ) );
			actions[1] = new EventWithKineticRate( "ReleaseTubulin", ReleaseTubulin, kinesin.kineticRates.GetRate( "F" ) );
			_actionsForState.Add( MotorState.MtKDP, actions );

			actions = new EventWithKineticRate[2];
			actions[0] = new EventWithKineticRate( "ReleaseADP", ReleaseADP, kinesin.kineticRates.GetRate( "J" ) );
			actions[1] = new EventWithKineticRate( "ReleaseTubulin", ReleaseTubulin, kinesin.kineticRates.GetRate( "I" ) );
			_actionsForState.Add( MotorState.MtKD, actions );

			actions = new EventWithKineticRate[1];
			// check bind tubulin via collision test during random walk
			actions[0] = new EventWithKineticRate( "ReleasePhosphate", ReleasePhosphate, kinesin.kineticRates.GetRate( "G" ) );
			_actionsForState.Add( MotorState.KDP, actions );

			actions = new EventWithKineticRate[0];
			// check bind tubulin via collision test during random walk
			_actionsForState.Add( MotorState.KD, actions );
		}

		public override void Simulate ()
		{
			DoRandomWalk();

			if (needToSwitchToStrong)
			{
				TryToSwitchToStrong();
			}
			else
			{
				DoInRandomOrder( actionsForState[state] );
			}
		}

		void TryToSwitchToStrong ()
		{
			if (bound)
			{
				if (state == MotorState.MtKDP)
				{
					ReleasePhosphate();
				}
				else if (state == MotorState.MtKD)
				{
					ReleaseADP();
					needToSwitchToStrong = false;
				}
			}
			else if (!chargeForceFields.activeSelf)
			{
				chargeForceFields.SetActive( true );
			}
		}

		void BindATP ()
		{
			if (logEvents) { Debug.Log( name + " bind ATP --------------------------------" ); }
			state = MotorState.MtKT;
			ATP.SetActive( true );
			kinesin.hips.StartSnap( this );
		}

		void ReleaseATP ()
		{
			if (logEvents) { Debug.Log( name + " release ATP --------------------------------" ); }
			state = MotorState.MtK;
			ATP.SetActive( false );
		}

		void HydrolyzeATP ()
		{
			if (logEvents) { Debug.Log( name + " hydrolyze --------------------------------" ); }
			state = MotorState.MtKDP;
			ATP.SetActive( false );
			ADP.SetActive( true );
			Pi.SetActive( true );
			otherMotor.SwitchToStrong();
		}

		void SwitchToStrong ()
		{
			needToSwitchToStrong = stateIsStrong ? false : true;
		}

		void ReleasePhosphate ()
		{
			if (logEvents) { Debug.Log( name + " release Pi --------------------------------" ); }
			state = (state == MotorState.MtKDP) ? MotorState.MtKD : MotorState.KD;
			Pi.SetActive( false );
		}

		void ReleaseADP ()
		{
			if (!otherMotor.stateIsStrong)
			{
				if (logEvents) { Debug.Log( name + " release ADP --------------------------------" ); }
				state = MotorState.MtK;
				ADP.SetActive( false );
			}
		}

		// --------------------------------------------------------------------------------------------------- Random walk

		protected override void DoRandomWalk ()
		{
			if (!bound)
			{
				Rotate();
				for (int i = 0; i < kinesin.maxIterationsPerStep; i++)
				{
					if (Move() || bound)
					{
						return;
					}
				}
				Jitter( 0.1f );
			}
			else 
			{
				Jitter();

				if (Time.time - lastSetToBindingPositionTime >= 1f)
				{
					transform.position = tubulin.transform.TransformPoint( bindingPosition );
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

		protected override void ProcessHits (RaycastHit[] hits)
		{
			CheckForTubulinCollision( hits );
		}

		void CheckForTubulinCollision (RaycastHit[] hits)
		{
			if (!bound && (needToSwitchToStrong || Time.time - lastReleaseTime > 1f))
			{
				Tubulin t;
				List<Tubulin> tubulins = new List<Tubulin>();
				foreach (RaycastHit hit in hits)
				{
					t = hit.collider.GetComponentInParent<Tubulin>();
					if (t != null && t.type == 1 && !t.hasMotorBound)
					{
						tubulins.Add( t );
					}
				}

				if (tubulins.Count > 0)
				{
					t = FindClosestValidTubulin( tubulins );
					if (t != null)
					{
						BindTubulin( t );
					}
				}
			}
		}

		Tubulin FindClosestValidTubulin (List<Tubulin> tubulins)
		{
			float d, hipsD, minDistance = Mathf.Infinity;
			Vector3 _bindingPosition;
			Tubulin closestTubulin = null;
			foreach (Tubulin t in tubulins)
			{
				_bindingPosition = t.transform.TransformPoint( bindingPosition );
				hipsD = Vector3.Distance( _bindingPosition, kinesin.hips.transform.position );
				d = Vector3.Distance( _bindingPosition, transform.position );
				if (hipsD <= maxDistanceFromParent && d < minDistance) // && closeToBindingOrientation( t )
				{
					minDistance = d;
					closestTubulin = t;
				}
			}
			return closestTubulin;
		}

		bool closeToBindingOrientation (Tubulin _tubulin)
		{
			Vector3 localRotation = (Quaternion.Inverse( _tubulin.transform.rotation ) * transform.rotation).eulerAngles;
			return Helpers.AngleIsWithinTolerance( localRotation.x, bindingRotation.x, bindingRotationTolerance )
				&& Helpers.AngleIsWithinTolerance( localRotation.y, bindingRotation.y, bindingRotationTolerance )
				&& Helpers.AngleIsWithinTolerance( localRotation.z, bindingRotation.z, bindingRotationTolerance );
		}

		void BindTubulin (Tubulin _tubulin)
		{
			if (logEvents) { Debug.Log( name + " BIND --------------------------------" ); }
			state = (state == MotorState.KDP) ? MotorState.MtKDP : MotorState.MtKD;
			tubulin = _tubulin;
			tubulin.hasMotorBound = true;
			transform.rotation = tubulin.transform.rotation * Quaternion.Euler( bindingRotation );
			transform.position = tubulin.transform.TransformPoint( bindingPosition );
			lastSetToBindingPositionTime = Time.time;
			kinesin.SetParentSchemeOnBind( this );
			chargeForceFields.SetActive( false );
		}

		void ReleaseTubulin ()
		{
			if (otherMotor.bound)
			{
				if (logEvents) { Debug.Log( name + " RELEASE --------------------------------" ); }
				state = (state == MotorState.MtKDP) ? MotorState.KDP : MotorState.KD;
				kinesin.SetParentSchemeOnRelease( this );
				lastReleaseTime = Time.time;
				if (tubulin != null)
				{
					tubulin.hasMotorBound = false;
					Vector3 fromTubulin = (transform.position - tubulin.transform.position).normalized;
					MoveIfValid( fromTubulin );
				}
				kinesin.hips.SetFree( this );
				chargeForceFields.SetActive( true );
			}
		}

		// --------------------------------------------------------------------------------------------------- Reset

		public override void Reset ()
		{
			if (logEvents) { Debug.Log( name + " reset --------------------------------" ); }
			state = MotorState.KD;
			needToSwitchToStrong = false;
			tubulin = null;
			lastReleaseTime = -1f;
			ATP.SetActive( false );
			ADP.SetActive( true );
			Pi.SetActive( false );
			chargeForceFields.SetActive( true );
		}
	}
}