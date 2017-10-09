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
					_lastCachedMotorEvents = new List<CachedMotorEvent>();
				}
				return _lastCachedMotorEvents;
			}
		}

		void InitCache ()
		{
			eventQueue.Clear();
			eventList.Clear();
			lastCachedMotorEvents.Clear();

			SeedCache();
			IncreaseCache( nanosecondsToCacheAtStart - lastCachedTime );
		}

		void SeedCache ()
		{
			for (int i = 0; i < motors.Count; i++)
			{
				lastCachedMotorEvents.Add( new CachedMotorEvent( motors[i], MotorState.KDP, MotorState.KD, 0 ) );
			}
			for (int i = 0; i < motors.Count; i++)
			{
				lastCachedMotorEvents[i] = GetNextEventToCache( i );
			}
			lastCachedMotorEvents.Sort(); //so that earliest is first
//			Debug.Log( "SEED " + lastCachedMotorEvents[0].motor.name + " : " + lastCachedMotorEvents[0].startState + " --> " + lastCachedMotorEvents[0].finalState
//				+ " @ " + lastCachedMotorEvents[0].timeNanoseconds );
			eventQueue.Enqueue( lastCachedMotorEvents[0] );
			eventList.Add( lastCachedMotorEvents[0] );
			lastCachedTime = lastCachedMotorEvents[0].timeNanoseconds;
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
			int lastIndex = lastMotorEventIndex;
			int firstIndex = OtherIndex( lastIndex );

			lastCachedMotorEvents[firstIndex] = GetNextEventToCache( firstIndex );

			while (lastCachedMotorEvents[firstIndex].timeNanoseconds < lastCachedMotorEvents[lastIndex].timeNanoseconds)
			{
//				Debug.Log( "EXTRA " + lastCachedMotorEvents[firstIndex].motor.name + " : " + lastCachedMotorEvents[firstIndex].startState + " --> " + lastCachedMotorEvents[firstIndex].finalState
//					+ " @ " + lastCachedMotorEvents[firstIndex].timeNanoseconds );
				eventQueue.Enqueue( lastCachedMotorEvents[firstIndex] );
				eventList.Add( lastCachedMotorEvents[firstIndex] );

				lastCachedMotorEvents[firstIndex] = GetNextEventToCache( firstIndex );
			}
//			Debug.Log( lastCachedMotorEvents[lastIndex].motor.name + " : " + lastCachedMotorEvents[lastIndex].startState + " --> " + lastCachedMotorEvents[lastIndex].finalState
//				+ " @ " + lastCachedMotorEvents[lastIndex].timeNanoseconds );
			eventQueue.Enqueue( lastCachedMotorEvents[lastIndex] );
			eventList.Add( lastCachedMotorEvents[lastIndex] );

			return lastCachedMotorEvents[lastIndex].timeNanoseconds;
		}

		int lastMotorEventIndex
		{
			get
			{
				return lastCachedMotorEvents[0].CompareTo( lastCachedMotorEvents[1] ) > 0 ? 0 : 1;
			}
		}

		int OtherIndex (int motorIndex)
		{
			return Mathf.Abs( motorIndex - 1 );
		}

		CachedMotorEvent GetNextEventToCache (int index)
		{
			CachedMotorEvent lastEvent = lastCachedMotorEvents[index];
			Kinetic nextEvent = lastEvent.motor.GetNextEvent( lastEvent.finalState, lastCachedMotorEvents[OtherIndex( index )].startState );
			float nextEventTime = lastEvent.motor.GetEventTime( nextEvent, lastEvent.timeNanoseconds );
			return new CachedMotorEvent( lastEvent.motor, lastEvent.finalState, (MotorState)nextEvent.finalStateIndex, nextEventTime );
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