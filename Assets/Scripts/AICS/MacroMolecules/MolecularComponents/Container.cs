using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class Container : MolecularComponent, IValidateMoves
	{
		public bool MoveIsValid (Vector3 position, float radius)
		{
			Molecule[] children = molecule.GetComponentsInChildren<Molecule>();
			foreach (Molecule child in children)
			{
				if (!MolecularEnvironment.Instance.PointIsInBounds( position + molecule.transform.InverseTransformPoint( child.transform.position ) ))
				{
					return false;
				}
			}
			return MolecularEnvironment.Instance.PointIsInBounds( position );
		}
	}
}