using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class NullMoleculeBinder : MoleculeBinder
	{
		public MoleculeType _typeToBind;

		protected override MoleculeBinder GetBinderToBind ()
		{
			return null;
		}
	}
}
