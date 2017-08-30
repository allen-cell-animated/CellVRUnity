using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;
using System.IO;

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
		public bool logEvents;
		public MotorState state = MotorState.KD;
		public float bindingRotationTolerance = 30f;
		public GameObject ATP;
		public GameObject ADP;
		public GameObject Pi;
		public GameObject chargeForceFields;
		public bool needToSwitchToStrong;
		public Kinetics kinetics;

		List<Tubulin> collidingTubulins = new List<Tubulin>();
		Vector3 bindingPosition = new Vector3( 0, 4.53f, 0 );
		Vector3 bindingRotation = new Vector3( 0, 0, 0 );
		Tubulin tubulin;
		float lastReleaseTime = -1f;
		float lastSetToBindingPositionTime = -1f;

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

		void Awake ()
		{
			interactsWithOtherMolecules = true;
		}

		void Start ()
		{
			kinetics = new Kinetics( kinesin.kineticRates );

			for (int i = 0; i < 6; i++)
			{
				count.Add( 0 );
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
			actions[0] = new EventWithKineticRate( "BindATP", BindATP, kinetics.kinetics[0] );
			_actionsForState.Add( MotorState.MtK, actions );

			actions = new EventWithKineticRate[2];
			actions[0] = new EventWithKineticRate( "ReleaseATP", ReleaseATP, kinetics.kinetics[1] );
			actions[1] = new EventWithKineticRate( "HydrolyzeATP", HydrolyzeATP, kinetics.kinetics[2] );
			_actionsForState.Add( MotorState.MtKT, actions );

			actions = new EventWithKineticRate[2];
			actions[0] = new EventWithKineticRate( "ReleasePhosphate", ReleasePhosphate, kinetics.kinetics[3] );
			actions[1] = new EventWithKineticRate( "ReleaseTubulin", ReleaseTubulin, kinetics.kinetics[5] );
			_actionsForState.Add( MotorState.MtKDP, actions );

			actions = new EventWithKineticRate[2];
			actions[0] = new EventWithKineticRate( "ReleaseADP", ReleaseADP, kinetics.kinetics[9] );
			actions[1] = new EventWithKineticRate( "ReleaseTubulin", ReleaseTubulin, kinetics.kinetics[8] );
			_actionsForState.Add( MotorState.MtKD, actions );

			actions = new EventWithKineticRate[1];
			// check bind tubulin via collision test during random walk
			actions[0] = new EventWithKineticRate( "ReleasePhosphate", ReleasePhosphate, kinetics.kinetics[6] );
			_actionsForState.Add( MotorState.KDP, actions );

			actions = new EventWithKineticRate[0];
			// check bind tubulin via collision test during random walk
			_actionsForState.Add( MotorState.KD, actions );
		}

		public override void DoCustomSimulation ()
		{
			count[(int)state]++;
			DoRandomWalk();

			if (needToSwitchToStrong)
			{
				TryToSwitchToStrong();
			}
			else
			{
				DoInRandomOrder( actionsForState[state] );
			}

			kinetics.CalculateObservedRates( nanosecondsSinceStart );
		}

		public List<int> count = new List<int>();
		string filePath = "/Users/blairl/Desktop/test";
		public int n;
		void OnApplicationQuit ()
		{
			string data = " , " + MolecularEnvironment.Instance.timeMultiplier + "\n" + " , " + kinesin.averageWalkingSpeed + "\n";
			int r = 0;
			foreach (Kinetic k in kinetics.kinetics)
			{
				if (k.attempts < 1.1f * k.events && k.observedKineticRate < 0.9f * k.kineticRate.rate)
				{
					data += r + ", 1\n";
				}
				else 
				{
					data += r + ", 0\n";
				}
				r++;
			}
			data += "\n";
			for (int i = 0; i < 6; i++)
			{
				data += i + ", " + count[i] + "\n";
			}
			data += "\n";
			r = 0;
			foreach (Kinetic k in kinetics.kinetics)
			{
				data += r + ", " + (k.observedKineticRate / k.kineticRate.rate) + "\n";
			}

			File.WriteAllText( filePath + n.ToString() + ".csv", data );
		}

		void TryToSwitchToStrong ()
		{
			if (bound)
			{
				if (state == MotorState.MtKDP)
				{
					if (!kinetics.kinetics[3].observedRateTooHigh)
					{
						kinetics.kinetics[3].attempts++;
						ReleasePhosphate();
					}
				}
				else if (state == MotorState.MtKD)
				{
					if (!kinetics.kinetics[9].observedRateTooHigh)
					{
						kinetics.kinetics[9].attempts++;
						ReleaseADP();
						needToSwitchToStrong = false;
					}
				}
			}
			else 
			{
				if (!chargeForceFields.activeSelf)
				{
					chargeForceFields.SetActive( true );
				}
				if (state == MotorState.KDP)
				{
					if (!kinetics.kinetics[6].observedRateTooHigh)
					{
						kinetics.kinetics[6].attempts++;
						ReleasePhosphate();
					}
				}
			}
		}

		void BindATP ()
		{
			kinetics.kinetics[0].events++;
			if (logEvents) { Debug.Log( name + " bind ATP --------------------------------" ); }

			state = MotorState.MtKT;
			ATP.SetActive( true );
			kinesin.hips.StartSnap( this );
		}

		void ReleaseATP ()
		{
			kinetics.kinetics[1].events++;
			if (logEvents) { Debug.Log( name + " release ATP --------------------------------" ); }

			state = MotorState.MtK;
			ATP.SetActive( false );
		}

		void HydrolyzeATP ()
		{
			kinetics.kinetics[2].events++;
			if (logEvents) { Debug.Log( name + " hydrolyze --------------------------------" ); }

			state = MotorState.MtKDP;
			ATP.SetActive( false );
			ADP.SetActive( true );
			Pi.SetActive( true );
			otherMotor.SwitchToStrong();
		}

		void SwitchToStrong ()
		{
			needToSwitchToStrong = !stateIsStrong;
		}

		void ReleasePhosphate ()
		{			
			kinetics.kinetics[bound ? 3 : 6].events++;
			if (logEvents) { Debug.Log( name + " release Pi --------------------------------" ); }

			state = (state == MotorState.MtKDP) ? MotorState.MtKD : MotorState.KD;
			Pi.SetActive( false );
		}

		void ReleaseADP ()
		{
			if (!otherMotor.stateIsStrong)
			{
				kinetics.kinetics[9].events++;
				if (logEvents) { Debug.Log( name + " release ADP --------------------------------" ); }

				state = MotorState.MtK;
				ADP.SetActive( false );
			}
		}

		// --------------------------------------------------------------------------------------------------- Random walk

		public override void DoRandomWalk ()
		{
			if (!bound)
			{
				Rotate();
				for (int i = 0; i < MolecularEnvironment.Instance.maxIterationsPerStep; i++)
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

		protected override void InteractWithCollidingMolecules ()
		{
			CheckForTubulinCollision( collidingMolecules );
		}

		void CheckForTubulinCollision (List<Molecule> collidingMolecules)
		{
			if (!bound && (needToSwitchToStrong || Time.time - lastReleaseTime > 0.1f))
			{
				Tubulin t;
				collidingTubulins.Clear();
				foreach (Molecule m in collidingMolecules)
				{
					t = m as Tubulin;
					if (t != null && t.type == 1 && !t.hasMotorBound)
					{
						collidingTubulins.Add( t );
					}
				}

				if (collidingTubulins.Count > 0)
				{
					t = FindClosestValidTubulin( collidingTubulins );
					if (t != null)
					{
						kinetics.kinetics[state == MotorState.KDP ? 4 : 7].attempts++;
						if (shouldBind)
						{
							BindTubulin( t );
						}
					}
				}
			}
		}

		bool shouldBind
		{
			get
			{
				return kinetics.kinetics[(state == MotorState.KDP) ? 4 : 7].ShouldHappen( MolecularEnvironment.Instance.nanosecondsPerStep, stepsSinceStart );
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
			kinetics.kinetics[state == MotorState.KDP ? 4 : 7].events++;
			if (logEvents) { Debug.Log( name + " BIND --------------------------------" ); }

			state = (state == MotorState.KDP) ? MotorState.MtKDP : MotorState.MtKD;
			tubulin = _tubulin;
			tubulin.hasMotorBound = true;
			transform.rotation = tubulin.transform.rotation * Quaternion.Euler( bindingRotation );
			transform.position = tubulin.transform.TransformPoint( bindingPosition );
			lastSetToBindingPositionTime = Time.time;
			kinesin.SetParentSchemeOnComponentBind( this as ComponentMolecule );
			chargeForceFields.SetActive( false );
		}

		void ReleaseTubulin ()
		{
			if (otherMotor.bound)
			{
				kinetics.kinetics[state == MotorState.MtKDP ? 5 : 8].events++;
				if (logEvents) { Debug.Log( name + " RELEASE --------------------------------" ); }

				state = (state == MotorState.MtKDP) ? MotorState.KDP : MotorState.KD;
				kinesin.SetParentSchemeOnComponentRelease( this as ComponentMolecule );
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

		public override void DoCustomReset ()
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
			kinetics.Reset();

			for (int i = 0; i < 6; i++)
			{
				count[i] = 0;
			}
		}
	}
}