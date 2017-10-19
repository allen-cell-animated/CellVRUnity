using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class AssemblyMolecule : Molecule 
	{
		public List<ComponentMolecule> componentMolecules = new List<ComponentMolecule>();
		public ComponentMolecule defaultParent;

		public void SetParentSchemeOnComponentBind (ComponentMolecule _molecule)
		{
			_molecule.transform.SetParent( transform );
			SetParentRecursively( _molecule, null );
		}

		public void SetParentSchemeOnComponentRelease ()
		{
			if (defaultParent != null)
			{
				defaultParent.transform.SetParent( transform );
				SetParentRecursively( defaultParent, null );
			}
		}

		void SetParentRecursively (Molecule parent, Molecule grandparent)
		{
			List<Leash> leashes = parent.GetLeashes();
			foreach (Leash leash in leashes)
			{
				if (leash.attachedMolecule != grandparent)
				{
					leash.attachedMolecule.transform.SetParent( parent.transform );
					SetParentRecursively( leash.attachedMolecule, parent );
				}
			}
		}
	}
}