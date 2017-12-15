using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MacroMolecules;

namespace AICS
{
	public class TestStateMachine : TestComponent 
	{
		public StateMachine stateMachine;
		public int goalStateID;

		protected override void TestUntilPass ()
		{
			if (stateMachine.currentState.id == goalStateID)
			{
//				IntegrationTest.Pass();
			}
		}

		protected override void TestOnce () {}
	}
}