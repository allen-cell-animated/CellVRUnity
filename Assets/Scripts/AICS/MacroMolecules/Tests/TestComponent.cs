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
				Test();
				tested = true;
			}
		}

		protected abstract void Test ();

		protected void PassTest (bool condition)
		{
			if (condition)
			{
				IntegrationTest.Pass();
			}
			else
			{
				IntegrationTest.Fail();
			}
		}
	}
}