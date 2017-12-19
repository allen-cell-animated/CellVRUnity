using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MacroMolecules;

namespace AICS
{
	[System.Serializable]
	public class ComponentBranches
	{
		public ComponentMolecule molecule;
		public int branches;
	}

	public class TestBranches : TestComponent 
	{
		public AssemblyMolecule assembly;
		public ComponentBranches[] branchesToRoot;

		float b;
		protected override void TestOnce ()
		{
			foreach (ComponentBranches component in branchesToRoot)
			{
				b = 0;//assembly.GetMinBranchesToRoot( component.molecule, null );
				if (b != component.branches)
				{
					Debug.Log( component.molecule.name + " branches != " + b ); 
//					IntegrationTest.Fail();
				}
			}
//			IntegrationTest.Pass();
		}

		protected override void TestUntilPass () {}
	}
}