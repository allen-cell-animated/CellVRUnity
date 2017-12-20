using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class Monomer : Substrate
	{
		public Polymer polymer;

		public override void ParentToBoundMolecule (Molecule _bindingMolecule)
		{
			if (polymer != null)
			{
				polymer.ParentToBoundMolecule( _bindingMolecule );
				polymer.UpdateParentScheme();
			}
			else
			{
				base.ParentToBoundMolecule( _bindingMolecule );
			}
		}

		public override void UnParentFromReleasedMolecule (Molecule _releasingMolecule)
		{
			if (polymer != null)
			{
				polymer.UnParentFromReleasedMolecule( _releasingMolecule );
				polymer.UpdateParentScheme();
			}
			else
			{
				base.UnParentFromReleasedMolecule( _releasingMolecule );
			}
		}

		public override void SetToBindingOrientation (BindingSite bindingSite)
		{
			if (polymer != null)
			{
				polymer.SetToBindingOrientation( bindingSite );
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
			}
			else
			{
				base.SetToBindingOrientation( bindingSite );
			}
		}
	}
}