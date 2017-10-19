using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class BoolConditional : Conditional
	{
		public bool condition;

		protected override bool DoCheck ()
		{
			return condition;
		}
	}
}