using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public interface IBind 
	{
		MoleculeType typeToBind { get; }

		Molecule boundMolecule { get; }
	}
}