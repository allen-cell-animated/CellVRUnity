using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	[System.Serializable]
	public class State
	{
		public string name;
		public int id;
		public StateTransition[] transitions;

		public void CalculateObservedRates (float secondsSinceStart)
		{
			foreach (StateTransition transition in transitions)
			{
				transition.CalculateObservedRate( secondsSinceStart );
			}
		}

		public void Reset ()
		{
			foreach (StateTransition transition in transitions)
			{
				transition.Reset();
			}
		}
	}

	[System.Serializable]
	public class StateTransition
	{
		public string name;
		public int startStateID;
		public int finalStateID;
		public int attempts;
		public int events;
		public float theoreticalRate;
		public float observedRate;
		public MolecularComponent transitionComponent;
		public string transitionEventName;
		public TransitionEvent transitionEvent;

		public void CreateTransitionEvent ()
		{
			transitionEvent = System.Delegate.CreateDelegate( typeof(TransitionEvent), transitionComponent, transitionEventName ) as TransitionEvent;
		}

		public bool observedRateTooHigh
		{
			get
			{
				return observedRate > 1.2f * theoreticalRate;
			}
		}

		public bool observedRateTooLow
		{
			get
			{
				return observedRate < 0.8f * theoreticalRate;
			}
		}

		public bool ShouldHappen ()
		{
			if (observedRateTooLow)
			{
				return true;
			}
			if (observedRateTooHigh)
			{
				return false;
			}
			return Random.value <= theoreticalRate * MolecularEnvironment.Instance.nanosecondsSinceStart * 1E-9f / attempts;
		}

		public void CalculateObservedRate (float secondsSinceStart)
		{
			observedRate = Mathf.Round( events / secondsSinceStart );
		}

		public void Reset ()
		{
			events = attempts = 0;
			observedRate = 0;
		}
	}

	public delegate bool TransitionEvent ();

	public class StateMachine : MolecularComponent, ISimulate
	{
		public State currentState;
		public List<State> states;

		public void DoSimulationStep ()
		{
			TryTransitionsInRandomOrder();

			CalculateObservedRates();
		}

		void TryTransitionsInRandomOrder ()
		{
			if (currentState.transitions.Length > 0)
			{
				currentState.transitions.Shuffle();
				for (int i = 0; i < currentState.transitions.Length; i++)
				{
					if (DoTransitionAtKineticRate( currentState.transitions[i] ))
					{
						return;
					}
				}
			}
		}

		bool DoTransitionAtKineticRate (StateTransition transition)
		{
			transition.attempts++;
			if (transition.ShouldHappen())
			{
				if (transition.transitionEvent())
				{
					transition.events++;
					currentState = GetStateForID( transition.finalStateID );
					return true;
				}
			}
			return false;
		}

		State GetStateForID (int id)
		{
			return states.Find( s => s.id == id );
		}

		void CalculateObservedRates ()
		{
			float secondsSinceStart = MolecularEnvironment.Instance.nanosecondsSinceStart * 1E-9f;
			foreach (State state in states)
			{
				state.CalculateObservedRates( secondsSinceStart );
			}
		}

		public void Reset ()
		{
			foreach (State state in states)
			{
				state.Reset();
			}
		}
	}
}