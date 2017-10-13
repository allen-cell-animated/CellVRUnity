using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class AssemblyMolecule : Molecule 
	{
		public List<ComponentMolecule> componentMolecules = new List<ComponentMolecule>();

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