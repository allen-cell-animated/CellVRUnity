using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AICS.MacroMolecules
{
	[System.Serializable]
	public class State
	{
		public string name;
		public int id;
		public bool startState;
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

		public void EnterState ()
		{
			foreach (StateTransition transition in transitions)
			{
				transition.EnterStartState();
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
		public int successes;
		public bool useRate = true;
		public float theoreticalRate;
		public float timeToTransition;
		public float observedRate;
		public Conditional[] conditionals;
		public UnityEvent eventToDo;

		float startTime;

		bool observedRateTooHigh
		{
			get
			{
				return observedRate > 1.2f * theoreticalRate;
			}
		}

		bool observedRateTooLow
		{
			get
			{
				return observedRate < 0.8f * theoreticalRate;
			}
		}

		public void EnterStartState ()
		{
			if (!useRate)
			{
				startTime = Time.time;
			}
		}

		public bool ShouldHappen ()
		{
			if (useRate)
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
			else
			{
				return Time.time - startTime >= timeToTransition;
			}
		}

		public bool PassesConditions ()
		{
			foreach (Conditional conditional in conditionals)
			{
				if (!conditional.Pass())
				{
					return false;
				}
			}
			return true;
		}

		public void DoTransition ()
		{
			eventToDo.Invoke();
			successes++;
		}

		public void CalculateObservedRate (float secondsSinceStart)
		{
			observedRate = Mathf.Round( successes / secondsSinceStart );
		}

		public void Reset ()
		{
			successes = attempts = 0;
			observedRate = 0;
		}
	}

	public class StateMachine : MolecularComponent, ISimulate
	{
		public State currentState;
		public List<State> states;

		void Start ()
		{
			foreach (State state in states)
			{
				if (state.startState)
				{
					currentState = state;
					return;
				}
			}
		}

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
			if (!transition.PassesConditions())
			{
				return false;
			}
			if (transition.eventToDo != null)
			{
				transition.attempts++;
				if (transition.ShouldHappen())
				{
					transition.DoTransition();
					currentState = GetStateForID( transition.finalStateID );
					currentState.EnterState();
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