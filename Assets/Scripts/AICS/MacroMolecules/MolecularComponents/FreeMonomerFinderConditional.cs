using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class FreeMonomerFinderConditional : FinderConditional
	{
		protected override bool MoleculeIsValid (Molecule _molecule)
		{
			return _molecule.polymer == null;
		}
	}
}