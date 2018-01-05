using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class OutsidePolymerFinderConditional : FinderConditional
	{
		protected override bool MoleculeIsValid (Molecule _molecule)
		{
			return _molecule.polymer == null || _molecule.polymer != molecule.polymer;
		}
	}
}