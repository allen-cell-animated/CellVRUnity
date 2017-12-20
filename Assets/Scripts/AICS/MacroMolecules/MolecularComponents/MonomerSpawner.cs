using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	[RequireComponent( typeof(Polymer) )]
	public class MonomerSpawner : MolecularComponent, ISetup
	{
		public List<Molecule> monomerPrefabs = new List<Molecule>();

		int n = 0;

		Polymer _polymer;
		Polymer polymer
		{
			get
			{
				if (_polymer == null)
				{
					_polymer = GetComponent<Polymer>();
				}
				return _polymer;
			}
		}

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
			foreach (Molecule _molecule in monomerPrefabs)
			{
				SpawnMonomer( _molecule );
			}
		}

		protected Molecule SpawnMonomer (Molecule prefab)
		{
			if (prefab != null)
			{
				Molecule _molecule = Instantiate( prefab, moleculeParent );
				_molecule.name = prefab.name + "_" + n;
				_molecule.polymer = polymer;
				n++;
				return _molecule;
			}
			return null;
		}

		protected void PlaceMonomer (Molecule _molecule, Vector3 position, Vector3 lookDirection, Vector3 normal)
		{
			_molecule.transform.localPosition = position;
			_molecule.transform.LookAt( _molecule.transform.position + lookDirection, normal );
		}
	}
}