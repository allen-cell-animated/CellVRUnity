using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class LeashConditional : Conditional
	{
		public Leash leash;
		public MoleculeBinder binder;

		protected override bool DoCheck ()
		{
			return true;
		}
	}
}