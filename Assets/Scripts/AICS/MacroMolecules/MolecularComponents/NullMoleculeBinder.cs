using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class NullMoleculeBinder : MoleculeBinder
	{
		public MoleculeType _typeToBind;

		public override MoleculeType typeToBind
		{
			get
			{
				return _typeToBind;
			}
		}

		protected override BindingSite GetSiteToBind ()
		{
			return null;
		}
	}
}
