using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class MoleculeSpawner : MonoBehaviour 
	{
		public Molecule prefab;
		public int amount;

		Molecule[] molecules;

		void Start ()
		{
			Spawn();
		}

		void Spawn ()
		{
			molecules = new Molecule[amount];
			for (int i = 0; i < amount; i++)
			{
				molecules[i] = Instantiate( prefab, transform ) as Molecule;
				molecules[i].transform.position = MolecularEnvironment.Instance.GetRandomPointInBounds();
				molecules[i].transform.rotation = Random.rotation;
				molecules[i].name = prefab.name + "_" + i.ToString();
			}
			MolecularEnvironment.Instance.needToGetMolecules = true;
		}

		public void DoReset ()
		{
			foreach (Molecule molecule in molecules)
			{
				Destroy( molecule.gameObject );
			}
			Spawn();
		}
	}
}