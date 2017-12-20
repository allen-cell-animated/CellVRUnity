using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MacroMolecules;

namespace AICS
{
	public class TestFindFront : TestComponent 
	{
		public FrontFinderConditional finder;
		public Molecule moleculeToFindMost;
		public int attempts;

		int n = 0;
//		int foundGoal;
//		int foundOther;

		protected override void TestOnce ()
		{
			while (n < attempts)
			{
				AttemptToFind();
				n++;
			}

//			PassTest(foundGoal > foundOther);
		}

		void AttemptToFind ()
		{
			if (finder != null)
			{
//				MoleculeBinder found = finder.Find();
//				if (found != null && found.molecule == moleculeToFindMost)
//				{
//					foundGoal++;
//				}
//				else 
//				{
//					foundOther++;
//				}
			}
		}

		protected override void TestUntilPass () {}
	}
}