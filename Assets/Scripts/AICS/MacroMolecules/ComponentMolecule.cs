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
			assembly.SetParentSchemeOnComponentBind( this );
		}

		public override void UnParentFromBoundMolecule ()
		{
			assembly.UnParentFromBoundMolecule();
			assembly.SetParentSchemeOnComponentRelease();
		}

		public override void SetToBindingOrientation (Vector3 position, Quaternion rotation)
		{
			assembly.transform.position = position;
			assembly.transform.rotation = rotation;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}
	}
}