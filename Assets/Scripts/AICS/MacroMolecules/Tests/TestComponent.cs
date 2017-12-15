using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MacroMolecules;

namespace AICS
{
	public abstract class TestComponent : MonoBehaviour 
	{
		bool tested = false;

		void Update () 
		{
			if (!tested)
			{
				TestOnce();
				tested = true;
			}
			TestUntilPass();
		}

		protected abstract void TestOnce ();

		protected abstract void TestUntilPass ();

		protected void PassTest (bool condition)
		{
			if (condition)
			{
//				IntegrationTest.Pass();
			}
			else
			{
//				IntegrationTest.Fail();
			}
		}
	}
}