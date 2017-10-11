using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class MoleculeBinder : MolecularComponent, IBind
	{
		public MoleculeType typeToBind;
		public Vector3 bindingPosition;
		public Vector3 bindingRotation;
		public Molecule boundMolecule;

		public void Bind ()
		{
			transform.rotation = other.transform.rotation * Quaternion.Euler( bindingRotation );
			molecule.SetPosition( other.transform.TransformPoint( bindingPosition ) );
		}
	}
}
