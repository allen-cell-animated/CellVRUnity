using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MacroMolecules;

namespace AICS
{
	public class TestParentScheme : TestComponent 
	{
		public AssemblyMolecule assembly;
		public StateMachine stateMachine;
		public int goalStateID;
		public Transform goalParent;

		protected override void TestUntilPass ()
		{
			if (stateMachine.currentState.id == goalStateID)
			{
				if (goalParent.parent == assembly.transform)
				{
					IntegrationTest.Pass();
				}
				else
				{
					IntegrationTest.Fail();
				}
			}
		}

		protected override void TestOnce () {}
	}
}