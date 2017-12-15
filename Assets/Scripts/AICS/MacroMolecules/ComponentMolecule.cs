using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class ComponentMolecule : Molecule 
	{
		public AssemblyMolecule assembly;

		public override void ParentToBoundMolecule (Molecule _boundMolecule)
		{
			assembly.ParentToBoundMolecule( _boundMolecule );
			assembly.UpdateParentScheme();
		}

		public override void UnParentFromBoundMolecule ()
		{
			assembly.UnParentFromBoundMolecule();
			assembly.UpdateParentScheme();
		}

		public override void SetToBindingOrientation (MoleculeBinder binder)
		{
			assembly.transform.position = binder.boundBinder.molecule.transform.TransformPoint( binder.bindingPosition );
			assembly.transform.rotation = binder.boundBinder.molecule.transform.rotation * Quaternion.Euler( binder.bindingRotation );
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}
	}
}