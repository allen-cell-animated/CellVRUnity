using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class MoleculeDetector : MonoBehaviour 
	{
		public List<Molecule> nearbyMolecules = new List<Molecule>();
		public MoleculeType moleculeToFind;

		List<Molecule> collidingMolecules = new List<Molecule>();

		public MoleculeDetector Setup (MoleculeType _moleculeToFind, float radius)
		{
			moleculeToFind = _moleculeToFind;
			transform.localPosition = Vector3.zero;
			GetComponent<SphereCollider>().radius = radius;
			return this;
		}

		void OnTriggerEnter (Collider other)
		{
			Molecule molecule = other.GetComponentInParent<Molecule>();
			if (molecule != null && (moleculeToFind == MoleculeType.All || molecule.type == moleculeToFind) && !nearbyMolecules.Contains( molecule ))
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

		public List<Molecule> GetCollidingMolecules (Vector3 position, float radius)
		{
			collidingMolecules.Clear();
			foreach (Molecule m in nearbyMolecules)
			{
				if (Vector3.Distance( m.transform.position, position ) <= m.radius + radius)
				{
					collidingMolecules.Add( m );
				}
			}
			return collidingMolecules;
		}

		public bool WillCollide (Vector3 position, float radius)
		{
			return GetCollidingMolecules( position, radius ).Count > 0;
		}
	}
}