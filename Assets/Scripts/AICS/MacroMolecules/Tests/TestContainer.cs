using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MacroMolecules;

namespace AICS
{
	public class TestContainer : TestComponent 
	{
		public float testDuration = 2f;
		public Molecule molecule;

		protected override void TestOnce () {}

		protected override void TestUntilPass () 
		{
			if (Time.time < testDuration)
			{
				if (!MolecularEnvironment.Instance.PointIsInBounds( molecule.transform.position ))
				{
					IntegrationTest.Fail();
				}
			}
			else
			{
				IntegrationTest.Pass();
			}
		}
	}
}