using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class StateConditional : Conditional
	{
		public FinderConditional finder;
		public int requiredStateIndex = 0;

		protected override bool DoCheck ()
		{
			StateMachine otherStateMachine = GetOtherStateMachine();
			if (otherStateMachine != null)
			{
				return otherStateMachine.currentState.id == requiredStateIndex;
			}
			return false;
		}

		StateMachine GetOtherStateMachine ()
		{
			if (finder.lastBinderFound != null)
			{
				List<ISimulate> simulators = finder.lastBinderFound.molecule.simulators;
				foreach (ISimulate simulator in simulators)
				{
					if (simulator.GetType() == typeof(StateMachine))
					{
						return simulator as StateMachine;
					}
				}
			}
			return null;
		}
	}
}