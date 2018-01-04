using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class Monomer : MolecularComponent
	{
		public virtual void SetToBindingOrientation (MoleculeBinder binder)
		{
			molecule.transform.position = binder.boundBinder.molecule.transform.TransformPoint( binder.bindingPosition );
			molecule.transform.rotation = binder.boundBinder.molecule.transform.rotation * Quaternion.Euler( binder.bindingRotation );
		}
	}
}