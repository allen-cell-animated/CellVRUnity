using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;
using System.IO;
using AICS.UI;

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
		public GameObject ATP;
		public GameObject ADP;
		public GameObject Pi;
		public bool needToSwitchToStrong;
		public StateIndicator stateIndicatorUI;
		public Direction forwardDirection;
		public Direction upDirection;
		public Kinetics kinetics;

		Vector3 bindingPosition = new Vector3( 0, 4.53f, 0 );
		Vector3 bindingRotation = new Vector3( 0, 0, 0 );
		Tubulin tubulin;
		float lastReleaseTime = -1f;
		float lastSetToBindingPositionTime = -1f;
		public Color tubulinFlashColor;

		MoleculeGraph<Tubulin> tubulinGraph;

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

		void SetState (MotorState newState)
		{
			state = newState;
			stateIndicatorUI.GoToState( (int)state );
		}

		protected override void OnAwake () { }

		public static Kinetic GetNextEvent (MotorState startState)
		{
			Kinetics[] possibleEvents = eventsForState[startState];
			float sumOfTheoreticalRates = GetSumOfTheoreticalRatesExitingState( startState );
			Kinetic eventToDo = null;

			do
			{
				eventToDo = TryToGetNextEvent( possibleEvents, sumOfTheoreticalRates );
			} 
			while (eventToDo == null);

			return eventToDo;
		}

		Kinetic TryToGetNextEvent (Kinetics[] possibleEvents, float sumOfTheoreticalRates)
		{
			possibleEvents.Shuffle();
			foreach (Kinetic k in possibleEvents)
			{
				if (Random.value <= k.theoreticalRate / sumOfTheoreticalRates)
				{
					return k;
				}
			}
			return null;
		}

		float GetSumOfTheoreticalRatesExitingState (MotorState startState)
		{
			float sum = 0;
			foreach (Kinetic k in eventsForState[startState])
			{
				sum += k.theoreticalRate;
			}
			return sum;
		}

		public static float GetEventNanoseconds (Kinetic _event)
		{
			float mean = 1f / _event.theoreticalRate;
			return Mathf.Max( 0, Helpers.SampleNormalDistribution( mean, mean / 3.5f ) );
		}

		void Start ()
		{
			kinetics = new Kinetics( kinesin.kineticRates );
			tubulinGraph = new MoleculeGraph<Tubulin>( forwardDirection, upDirection );

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
			count[(int)state]++;
			DoRandomWalk();

			if (needToSwitchToStrong)
			{
				TryToSwitchToStrong();
			}

			DoInRandomOrder( eventsForState[state] );

			kinetics.CalculateObservedRates();
		}

		public List<int> count = new List<int>();
//		string filePath = "/Users/blairl/Desktop/test";
//		public int n;
//		void OnApplicationQuit ()
//		{
//			string data = " , " + MolecularEnvironment.Instance.timeMultiplier + "\n" + " , " + kinesin.averageWalkingSpeed + "\n";
//			int r = 0;
//			foreach (Kinetic k in kinetics.kinetics)
//			{
//				if (k.attempts < 1.1f * k.events && k.observedRate < 0.9f * k.theoreticalRate)
//				{
//					data += r + ", 1\n";
//				}
//				else 
//				{
//					data += r + ", 0\n";
//				}
//				r++;
//			}
//			data += "\n";
//			for (int i = 0; i < 6; i++)
//			{
//				data += i + ", " + count[i] + "\n";
//			}
//			data += "\n";
//			r = 0;
//			foreach (Kinetic k in kinetics.kinetics)
//			{
//				data += r + ", " + (k.observedRate / k.theoreticalRate) + "\n";
//			}
//
//			File.WriteAllText( filePath + n.ToString() + ".csv", data );
//		}

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
			if (logEvents) { Debug.Log( name + " bind ATP --------------------------------" ); }

			SetState( (MotorState)kinetic.finalStateIndex );
			ATP.SetActive( true );
			kinesin.hips.StartSnap( this );
			return true;
		}

		bool ReleaseATP (Kinetic kinetic)
		{
			if (logEvents) { Debug.Log( name + " release ATP --------------------------------" ); }

			SetState( (MotorState)kinetic.finalStateIndex );
			ATP.SetActive( false );
			return true;
		}

		bool HydrolyzeATP (Kinetic kinetic)
		{
			if (logEvents) { Debug.Log( name + " hydrolyze --------------------------------" ); }

			SetState( (MotorState)kinetic.finalStateIndex );
			ATP.SetActive( false );
			ADP.SetActive( true );
			Pi.SetActive( true );
			otherMotor.SwitchToStrong();
			return true;
		}

		void SwitchToStrong ()
		{
			needToSwitchToStrong = !stateIsStrong;
		}

		bool ReleasePhosphate (Kinetic kinetic)
		{
			if (logEvents) { Debug.Log( name + " release Pi --------------------------------" ); }

			SetState( (MotorState)kinetic.finalStateIndex );
			Pi.SetActive( false );
			return true;
		}

		bool ReleaseADP (Kinetic kinetic)
		{
			if (!otherMotor.stateIsStrong)
			{
				if (logEvents) { Debug.Log( name + " release ADP --------------------------------" ); }

				SetState( (MotorState)kinetic.finalStateIndex );
				ADP.SetActive( false );
				return true;
			}
			return false;
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
			if (!bound && (Time.time - lastReleaseTime > 0.1f || needToSwitchToStrong))
			{
				Tubulin t = FindTubulin();
				if (t != null)
				{
					DoTubulinBind( t, kinetic );
					return true;
				}
			}
			return false;
		}

		Tubulin FindTubulin ()
		{
			if (otherMotor.tubulinGraph.empty)
			{
				return GetRandomValidTubulin();
			}
			else
			{
				return GetTubulinAroundOtherMotor();
			}
		}

		Tubulin GetRandomValidTubulin ()
		{
			List<Tubulin> tubulins = GetTubulins();
			List<Tubulin> validTubulins = new List<Tubulin>();
			foreach (Tubulin t in tubulins)
			{
				if (TubulinIsValid( t ))
				{
					validTubulins.Add( t );
				}
			}

			if (validTubulins.Count > 0)
			{
				return validTubulins[GetRandomIndex( validTubulins.Count )];
			}
			return null;
		}

		public List<MoleculeAngle> molecules = new List<MoleculeAngle>();

		Tubulin GetTubulinAroundOtherMotor ()
		{
			molecules.Clear();
			foreach (MoleculeAngle ma in otherMotor.tubulinGraph.molecules)
			{
				Tubulin t = ma.molecule as Tubulin;
				if (t != null && TubulinIsValid( t ))
				{
					molecules.Add( ma );
					t.Flash( tubulinFlashColor );
				}
			}

			if (molecules.Count > 0)
			{
				molecules.Sort();
				return molecules[GetExponentialRandomIndex( molecules.Count )].molecule as Tubulin;
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
			return _tubulin.tubulinType == 1 && !_tubulin.hasMotorBound && hipsDistance <= maxDistanceFromParent; // && CloseToBindingOrientation( t )
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
//			Debug.Log( (Mathf.CeilToInt( i * n ) - 1) + " / " + n );
			return Mathf.CeilToInt( i * n ) - 1;
//			return 0;
		}

		void DoTubulinBind (Tubulin _tubulin, Kinetic kinetic)
		{
			if (logEvents) { Debug.Log( name + " BIND --------------------------------" ); }

			SetState( (MotorState)kinetic.finalStateIndex );
			tubulin = _tubulin;
			kinesin.lastTubulin = tubulin;
			tubulin.hasMotorBound = true;
			transform.rotation = tubulin.transform.rotation * Quaternion.Euler( bindingRotation );
			SetPosition( tubulin.transform.TransformPoint( bindingPosition ) );
			lastSetToBindingPositionTime = Time.time;
			kinesin.SetParentSchemeOnComponentBind( this as ComponentMolecule );
			tubulinGraph.SetMolecules( GetTubulins(), transform );
		}

		bool ReleaseTubulin (Kinetic kinetic)
		{
			if (otherMotor.bound)
			{
				if (logEvents) { Debug.Log( name + " RELEASE --------------------------------" ); }

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
				return true;
			}
			return false;
		}

		protected override void InteractWithBindingPartners () { }

		// --------------------------------------------------------------------------------------------------- Reset

		public override void DoCustomReset ()
		{
			if (logEvents) { Debug.Log( name + " reset --------------------------------" ); }

			SetState( MotorState.KD );
			needToSwitchToStrong = false;
			tubulin = null;
			lastReleaseTime = -1f;
			ATP.SetActive( false );
			ADP.SetActive( true );
			Pi.SetActive( false );
			kinetics.Reset();
			tubulinGraph.Clear();

			for (int i = 0; i < 6; i++)
			{
				count[i] = 0;
			}
		}
	}
}