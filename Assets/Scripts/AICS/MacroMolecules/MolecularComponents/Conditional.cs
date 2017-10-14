using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class Conditional : MolecularComponent
	{
		public bool Pass ()
		{
			return DoCheck();
		}

		protected virtual bool DoCheck ()
		{
			return true;
		}
	}
}