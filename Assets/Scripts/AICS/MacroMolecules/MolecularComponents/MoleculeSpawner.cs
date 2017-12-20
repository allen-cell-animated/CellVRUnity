using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class MoleculeSpawner : MolecularComponent, ISetup
	{
		public List<Molecule> moleculePrefabs = new List<Molecule>();

		int n = 0;

		Transform _moleculeParent;
		protected Transform moleculeParent
		{
			get
			{
				if (_moleculeParent == null)
				{
					_moleculeParent = new GameObject( "Monomers" ).transform;
					_moleculeParent.SetParent( transform );
					_moleculeParent.localPosition = Vector3.zero;
					_moleculeParent.localRotation = Quaternion.identity;
				}
				return _moleculeParent;
			}
		}

		public void DoSetup ()
		{
			SpawnAll();
		}

		protected virtual void SpawnAll ()
		{
			foreach (Molecule _molecule in moleculePrefabs)
			{
				SpawnMolecule( _molecule );
			}
		}

		protected Molecule SpawnMolecule (Molecule prefab)
		{
			if (prefab != null)
			{
				Molecule _molecule = Instantiate( prefab, moleculeParent );
				_molecule.name = prefab.name + "_" + n;
				n++;
				return _molecule;
			}
			return null;
		}

		protected void PlaceMolecule (Molecule _molecule, Vector3 position, Vector3 lookDirection, Vector3 normal)
		{
			_molecule.transform.localPosition = position;
			_molecule.transform.LookAt( _molecule.transform.position + lookDirection, normal );
		}
	}
}