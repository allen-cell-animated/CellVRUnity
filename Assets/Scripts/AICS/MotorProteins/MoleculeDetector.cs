using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.MotorProteins
{
	public enum MoleculeType
	{
		None,
		Tubulin,
		KinesinMotor,
		KinesinHips,
		KinesinNecklinker,
		KinesinCargo,
		KinesinTropomyosin,
		Kinesin
	}

	// Keeps a list of nearby molecules (using layers to only find molecules of certain types) to check collisions against
	public class MoleculeDetector : MonoBehaviour 
	{
		public MoleculeType moleculeType;
		public List<Molecule> nearbyMolecules = new List<Molecule>();

		List<Molecule> collidingMolecules = new List<Molecule>();

		void OnTriggerEnter (Collider other)
		{
			Molecule molecule = other.GetComponentInParent<Molecule>();
			if (molecule != null && molecule.type == moleculeType && !nearbyMolecules.Contains( molecule ))
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

		public List<Molecule> GetCollidingMolecules (Molecule molecule, Vector3 moveStep, float radius = -1f)
		{
			collidingMolecules.Clear();
			foreach (Molecule m in nearbyMolecules)
			{
				if (Vector3.Distance( m.transform.position, molecule.transform.position + moveStep ) <= m.radius + (radius >= 0 ? radius : molecule.radius))
				{
					collidingMolecules.Add( m );
				}
			}
			return collidingMolecules;
		}
	}
}