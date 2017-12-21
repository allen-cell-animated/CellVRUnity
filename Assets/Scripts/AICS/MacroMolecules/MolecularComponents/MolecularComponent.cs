using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public abstract class MolecularComponent : MonoBehaviour 
	{
		public bool isEnabled = true;

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

		public void Enable (bool _enabled)
		{
			isEnabled = _enabled;
		}
	}
}