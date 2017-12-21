using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class MoleculeSpawner : MonoBehaviour 
	{
		public Molecule prefab;
		public int amount;

		void Start ()
		{
			Spawn();
		}

		void Spawn ()
		{
			Molecule m;
			for (int i = 0; i < amount; i++)
			{
				m = Instantiate( prefab, transform ) as Molecule;
				m.transform.position = MolecularEnvironment.Instance.GetRandomPointInBounds();
				m.transform.rotation = Random.rotation;
				m.name = prefab.name + "_" + i.ToString();
			}
		}
	}
}