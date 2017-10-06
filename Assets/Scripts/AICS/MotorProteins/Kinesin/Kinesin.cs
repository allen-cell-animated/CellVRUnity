using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.MotorProteins.Kinesin
{
	public class Kinesin : AssemblyMolecule 
	{
		public float averageWalkingSpeed; // μm/s
		public Tubulin lastTubulin;
		public KineticRates kineticRates;
		public float nanosecondsToCacheAtStart = 1E9f;

		public Queue<CachedMotorEvent> eventQueue = new Queue<CachedMotorEvent>();
		public List<CachedMotorEvent> eventList = new List<CachedMotorEvent>();
		float lastCachedTime;

		List<CachedMotorEvent> _lastCachedMotorEvents;
		List<CachedMotorEvent> lastCachedMotorEvents
		{
			get
			{
				if (_lastCachedMotorEvents == null)
				{
					InitCache();
				}
				return _lastCachedMotorEvents;
			}
		}

		void InitCache ()
		{
			eventQueue.Clear();
			eventList.Clear();
			lastCachedTime = 0;
			if (_lastCachedMotorEvents == null)
			{
				_lastCachedMotorEvents = new List<CachedMotorEvent>();
			}
			else
			{
				_lastCachedMotorEvents.Clear();
			}

			for (int i = 0; i < motors.Count; i++)
			{
				_lastCachedMotorEvents.Add( new CachedMotorEvent( motors[i], MotorState.KDP, MotorState.KD, 0 ) );
			}
			IncreaseCache( nanosecondsToCacheAtStart );
		}

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

		protected override void OnAwake ()
		{
			base.OnAwake();
			InitCache();
		}

		void IncreaseCache (float nanosecondsToAdd)
		{
			float goalTime = lastCachedTime + nanosecondsToAdd;
			while (lastCachedTime < goalTime)
			{
				lastCachedTime = CalculateCache();
			}
		}

		float CalculateCache () 
		{
			for (int i = 0; i < motors.Count; i++)
			{
				CachedMotorEvent lastEvent = lastCachedMotorEvents[i];
				Kinetic nextEvent = lastEvent.motor.GetNextEvent( lastEvent.finalState, lastCachedMotorEvents[Mathf.Abs(i - 1)].finalState );
				float nextEventTime = lastEvent.timeNanoseconds + lastEvent.motor.GetNanosecondsUntilEvent( nextEvent, lastEvent.timeNanoseconds );
				lastCachedMotorEvents[i] = new CachedMotorEvent( motors[i], (MotorState)lastEvent.finalState, 
					(MotorState)nextEvent.finalStateIndex, lastEvent.timeNanoseconds + nextEventTime );
			}

			lastCachedMotorEvents.Sort(); //so that earliest is first
			foreach (CachedMotorEvent _event in lastCachedMotorEvents)
			{
				eventQueue.Enqueue( _event );
				eventList.Add( _event );
			}

			return lastCachedMotorEvents[0].timeNanoseconds;
		}

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
			InitCache();
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