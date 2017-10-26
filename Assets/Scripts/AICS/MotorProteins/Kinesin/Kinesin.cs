using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;
using AICS.PhysicsKinesin;

namespace AICS.MotorProteins.Kinesin
{
	public class CachedMotorEvent
	{
		public MotorState startState;
		public MotorState finalState;
		public float timeNanoseconds; 

		public CachedMotorEvent (MotorState _startState, MotorState _finalState, float _timeNanoseconds)
		{
			startState = _startState;
			finalState = _finalState;
			timeNanoseconds = _timeNanoseconds;
		}
	}

	public class Kinesin : AssemblyMolecule 
	{
		public float averageWalkingSpeed; // μm/s
		public Tubulin lastTubulin;
		public KineticRates kineticRates;
		public NucleotideGenerator atpGenerator;

//		Queue<CachedMotorEvent> eventQueue = new Queue<CachedMotorEvent>();
//		float lastQueuedTime;
//		CachedMotorEvent[] lastQueuedMotorEvents = new CachedMotorEvent[2];

		Hips _hips;
		public Hips hips
		{
			get
			{
				if (_hips == null)
				{
					_hips = componentMolecules.Find( m => m.GetComponent<Hips>() != null ) as Hips;
				}
				return _hips;
			}
		}

		List<Motor> _motors;
		public List<Motor> motors
		{
			get
			{
				if (_motors == null)
				{
					_motors = new List<Motor>();
					foreach (ComponentMolecule molecule in componentMolecules)
					{
						Motor motor = molecule.GetComponent<Motor>();
						if (motor != null)
						{
							_motors.Add( motor );
						}
					}
				}
				return _motors;
			}
		}

		public override bool bound
		{
			get
			{
				foreach (Motor motor in motors)
				{
					if (motor.bound)
					{
						return true;
					}
				}
				return false;
			}
		}

//		void IncreaseCache (float nanosecondsToAdd)
//		{
//			float goalTime = lastQueuedTime + nanosecondsToAdd;
//			while (lastQueuedTime < goalTime)
//			{
//				lastQueuedTime = CalculateCache();
//			}
//		}
//
//		float CalculateCache () 
//		{ 
//			float firstEventNanoseconds = Mathf.Infinity;
//			int firstEventIndex = 0;
//			for (int i = 0; i < lastQueuedMotorEvents.Length; i++)
//			{
//				CachedMotorEvent lastEvent = lastQueuedMotorEvents[i];
//				Kinetic k = Motor.GetNextEvent( lastEvent.finalState );
//				float t = Motor.GetEventNanoseconds( k );
//				if (t < firstEventNanoseconds)
//				{
//					firstEventNanoseconds = t;
//					firstEventIndex = i;
//				}
////				lastQueuedMotorEvents[i] = new CachedMotorEvent( lastEvent.finalState,  );
//			}
//			return 0;
//		}

		void Update ()
		{
			for (int i = 0; i < MolecularEnvironment.Instance.stepsPerFrame; i++)
			{
				Simulate();
			}

			if (resetFrames > 0)
			{
				FinishReset();
			}

			CalculateWalkingSpeed();
		}

		public override void DoCustomSimulation ()
		{
			foreach (ComponentMolecule molecule in componentMolecules)
			{
				molecule.Simulate();
			}
		}

		public override void DoCustomReset ()
		{
			SetHipsAsParent();
			hips.Reset();
			foreach (Molecule molecule in componentMolecules)
			{
				if (molecule != hips)
				{
					molecule.Reset();
				}
			}
		}

		void FinishReset ()
		{
			resetFrames--;
			foreach (Molecule molecule in componentMolecules)
			{
				molecule.resetFrames = resetFrames;
			}
		}

		public override void SetParentSchemeOnComponentBind (ComponentMolecule molecule)
		{
			Motor motor = molecule as Motor;
			Motor otherMotor = motors.Find( m => m != motor );
			if (otherMotor.bound)
			{
				motor.transform.SetParent( transform );
				hips.SetSecondParent( motor.transform );
			}
			else
			{
				motor.transform.SetParent( transform );
				hips.transform.SetParent( motor.transform );
				otherMotor.transform.SetParent( hips.transform );
			}
			ResetComponentScale();
		}

		public override void SetParentSchemeOnComponentRelease (ComponentMolecule molecule)
		{
			Motor motor = molecule as Motor;
			Motor otherMotor = motors.Find( m => m != motor );
			if (otherMotor.bound)
			{
				hips.transform.SetParent( otherMotor.transform );
				motor.transform.SetParent( hips.transform );
			}
			else
			{
				SetHipsAsParent();
			}
			hips.SetSecondParent( null );
			ResetComponentScale();
		}

		void SetHipsAsParent ()
		{
			hips.transform.SetParent( transform );
			motors[0].transform.SetParent( hips.transform );
			motors[1].transform.SetParent( hips.transform );
		}

		void CalculateWalkingSpeed ()
		{
			averageWalkingSpeed = 1E-3f * (hips.transform.position - hips.startPosition).magnitude / (MolecularEnvironment.Instance.nanosecondsSinceStart * 1E-9f);
		}

		public void Print ()
		{
			foreach (Molecule molecule in componentMolecules)
			{
				float d = Mathf.Round( Vector3.Distance( molecule.startPosition, molecule.transform.position ) );
				if (d > 0)
				{
					Debug.Log( molecule.name + " " + d );
				}
			}
		}
	}
}