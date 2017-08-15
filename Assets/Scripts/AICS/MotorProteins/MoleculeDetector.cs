using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.MotorProteins
{
	// Keeps a list of nearby molecules (using layers to only find molecules of certain types) to check collisions against
	public class MoleculeDetector : MonoBehaviour 
	{
		public List<Molecule> nearbyMolecules = new List<Molecule>();

		List<Molecule> collidingMolecules = new List<Molecule>();

		void OnTriggerEnter (Collider other)
		{
			Molecule molecule = other.GetComponentInParent<Molecule>();
			if (molecule != null && !nearbyMolecules.Contains( molecule ))
			{
				nearbyMolecules.Add( molecule );
			}
		}

		void OnTriggerExit (Collider other)
		{
			Molecule molecule = other.GetComponentInParent<Molecule>();
			if (molecule != null && nearbyMolecules.Contains( molecule ))
			{
				nearbyMolecules.Remove( molecule );
			}
		}

		public List<Molecule> GetCollidingMolecules (Molecule molecule, Vector3 moveStep)
		{
			collidingMolecules.Clear();
			foreach (Molecule m in nearbyMolecules)
			{
				if (Vector3.Distance( m.transform.position, molecule.transform.position + moveStep ) <= m.radius + molecule.radius)
				{
					collidingMolecules.Add( m );
				}
			}
			return collidingMolecules;
		}
	}
}