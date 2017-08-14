using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.AnimatedKinesin
{
	public class MoleculeDetector : MonoBehaviour 
	{
		public List<Molecule> nearbyMolecules = new List<Molecule>();

		void OnTriggerEnter (Collider other)
		{
			Molecule molecule = other.GetComponent<Molecule>();
			if (molecule != null && !nearbyMolecules.Contains( molecule ))
			{
				nearbyMolecules.Add( molecule );
			}
		}

		void OnTriggerExit (Collider other)
		{
			Molecule molecule = other.GetComponent<Molecule>();
			if (molecule != null && nearbyMolecules.Contains( molecule ))
			{
				nearbyMolecules.Remove( molecule );
			}
		}

		public Molecule[] GetCollidingMolecules (Molecule molecule)
		{
			List<Molecule> collidingMolecules = new List<Molecule>();
			foreach (Molecule m in nearbyMolecules)
			{
				if (Vector3.Distance( m.transform.position, molecule.transform.position ) <= m.radius + molecule.radius)
				{
					collidingMolecules.Add( m );
				}
			}
			return collidingMolecules.ToArray();
		}
	}
}