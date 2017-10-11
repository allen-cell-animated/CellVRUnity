using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class MolecularAssembly : Molecule 
	{
		public List<Molecule> componentMolecules;

		public void SetParentSchemeOnComponentBind (ComponentMolecule _molecule)
		{
			//parse leashes between components
		}

		public void SetParentSchemeOnComponentRelease (ComponentMolecule _molecule)
		{
			//parse leashes between components
		}
	}
}