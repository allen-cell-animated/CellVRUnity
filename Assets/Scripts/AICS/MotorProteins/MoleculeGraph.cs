using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AICS.MotorProteins
{
	public class MoleculeAngle : IComparable<MoleculeAngle>
	{
		public Molecule molecule;
		public float angle;

		public MoleculeAngle (Molecule _molecule, float _angle)
		{
			molecule = _molecule;
			angle = _angle;
		}

		public int CompareTo (MoleculeAngle other)
		{
			if (other.angle < angle)
			{
				return -1;
			}
			else if (other.angle == angle)
			{
				return 0;
			}
			else 
			{
				return 1;
			}
		}
	}

	[System.Serializable]
	public class MoleculeGraph<T> where T : Molecule
	{
		public List<MoleculeAngle> molecules = new List<MoleculeAngle>();

		Direction forwardDirection;
		Direction upDirection;
		Transform lastCenterTransform;

		public MoleculeGraph (Direction _forwardDirection, Direction _upDirection)
		{
			forwardDirection = _forwardDirection;
			upDirection = _upDirection;
		}

		public void AddMolecules (List<T> _molecules, Transform centerTransform)
		{
			molecules.Clear();
			lastCenterTransform = centerTransform;
			Molecule molecule;
			float angle;
			foreach (T m in _molecules)
			{
				molecule = m as Molecule;
				angle = GetMoleculeAngleFromForward( molecule );
				molecules.Add( new MoleculeAngle( molecule, angle ) );
			}
		}

		// Get angle in degrees from forward vector
		float GetMoleculeAngleFromForward (Molecule molecule)
		{
			Vector3 toMolecule = molecule.transform.position - lastCenterTransform.position;
			Vector3 normal = Helpers.GetLocalDirection( upDirection, lastCenterTransform );
			Vector3 projectionToNormal = Vector3.Dot( toMolecule, normal ) * normal;
			Vector3 moleculeDirectionInMotorPlane = (toMolecule - projectionToNormal).normalized;

			return Mathf.Acos( Vector3.Dot( Helpers.GetLocalDirection( forwardDirection, lastCenterTransform ), moleculeDirectionInMotorPlane ) ) * Mathf.Rad2Deg;
		}
	}
}