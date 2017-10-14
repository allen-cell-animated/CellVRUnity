using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public abstract class MolecularComponent : MonoBehaviour 
	{
		Molecule _molecule;
		public Molecule molecule
		{
			get
			{
				if (_molecule == null)
				{
					_molecule = GetComponentInParent<Molecule>();
				}
				return _molecule;
			}
		}
	}
}