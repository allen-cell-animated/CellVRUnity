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
		[HideInInspector] public int[] transitionIDs;

		public void Init ()
		{
			transitionIDs = new int[transitions.Length];
			for (int i = 0; i < transitionIDs.Length; i++)
			{
				transitionIDs[i] = i;
			}
		}

		public void CalculateObservedRates (float secondsSinceStart)
		{
			foreach (StateTransition transition in transitions)
			{
				transition.CalculateObservedRate( secondsSinceStart );
			}
		}

		public void DoReset ()
		{
			foreach (StateTransition transition in transitions)
			{
				transition.DoReset();
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
		public bool triggered = false;
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
			else if (!triggered)
			{
				return Time.time - startTime >= timeToTransition;
			}
			return false;
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
			if (eventToDo != null)
			{
				eventToDo.Invoke();
			}
			successes++;
		}

		public void CalculateObservedRate (float secondsSinceStart)
		{
			observedRate = Mathf.Round( successes / secondsSinceStart );
		}

		public void DoReset ()
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
				state.Init();
				if (state.startState)
				{
					currentState = state;
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
			if (currentState == null) { Debug.Log( name ); }
			if (currentState.transitions.Length > 0)
			{
				currentState.transitionIDs.Shuffle();
				for (int i = 0; i < currentState.transitionIDs.Length; i++)
				{
					if (DoTransitionAtKineticRate( currentState.transitions[currentState.transitionIDs[i]] ))
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
			transition.attempts++;
			if (transition.ShouldHappen())
			{
				Debug.Log( molecule.name + " do transition " + transition.name );
				currentState = GetStateForID( transition.finalStateID );
				currentState.EnterState();
				transition.DoTransition();
				return true;
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

		public void ForceTransition (string stateTransitionID)
		{
			string[] ids = stateTransitionID.Split( ',' );
			int stateIndex = 0;
			int.TryParse( ids[0], out stateIndex );
			int transitionIndex = 0;
			int.TryParse( ids[1], out transitionIndex );

			if (currentState.id == stateIndex 
				&& transitionIndex >= 0 && transitionIndex < currentState.transitions.Length)
			{
				StateTransition transition = currentState.transitions[transitionIndex];
				if (transition != null)
				{
					Debug.Log( molecule.name + " force transition " + transition.name );
					transition.attempts++;
					currentState = GetStateForID( transition.finalStateID );
					transition.DoTransition();
					currentState.EnterState();
				}
			}
		}

		public void DoReset ()
		{
			foreach (State state in states)
			{
				state.DoReset();
			}
		}
	}
}