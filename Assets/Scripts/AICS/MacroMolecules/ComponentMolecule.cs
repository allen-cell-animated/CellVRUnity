using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class ComponentMolecule : Molecule 
	{
		public MolecularAssembly assembly;

		public override void ParentToBoundMolecule (Molecule _boundMolecule)
		{
			assembly.ParentToBoundMolecule( _boundMolecule );
			assembly.SetParentSchemeOnComponentBind( this );
		}

		public override void UnParentFromBoundMolecule ()
		{
			assembly.UnParentFromBoundMolecule();
			assembly.SetParentSchemeOnComponentRelease( this );
		}
	}
}