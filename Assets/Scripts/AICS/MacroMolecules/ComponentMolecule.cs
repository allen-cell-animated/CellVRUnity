using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class ComponentMolecule : Molecule 
	{
		public AssemblyMolecule assembly;

		public override void ParentToBoundMolecule (Molecule _bindingMolecule)
		{
			assembly.ParentToBoundMolecule( _bindingMolecule );
			assembly.UpdateParentScheme();
		}

		public override void UnParentFromBoundMolecule (Molecule _releasingMolecule)
		{
			assembly.UnParentFromBoundMolecule( _releasingMolecule );
			assembly.UpdateParentScheme();
		}

		public override void SetToBindingOrientation (MoleculeBinder binder)
		{
			assembly.SetToBindingOrientation( binder );
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}
	}
}