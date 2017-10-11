using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	[System.Serializable]
	public class MoleculeAngle : System.IComparable<MoleculeAngle>
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
			if (other.angle > angle)
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

	public class FrontMoleculeFinder : MoleculeFinder 
	{
		public Direction forwardDirection = Direction.forward;
		public Direction upDirection = Direction.up;
		public Transform centerTransform;

		List<MoleculeAngle> graphedMolecules = new List<MoleculeAngle>();

		protected override Molecule PickFromValidMolecules ()
		{
			GraphMolecules();
			return graphedMolecules[graphedMolecules.GetExponentialRandomIndex()];
		}

		void GraphMolecules ()
		{
			foreach (Molecule m in validMolecules)
			{
				graphedMolecules.Add( new MoleculeAngle( m, GetMoleculeAngleFromForward( m ) ) );
			}
			graphedMolecules.Sort();
		}

		float GetMoleculeAngleFromForward (Molecule molecule)
		{
			// Get angle in degrees from forward vector projected on plane passing through transform and perpendicular to up vector
			Vector3 toMolecule = molecule.transform.position - centerTransform.position;
			Vector3 normal = Helpers.GetLocalDirection( upDirection, centerTransform );
			Vector3 projectionToNormal = Vector3.Dot( toMolecule, normal ) * normal;
			Vector3 moleculeDirectionInMotorPlane = (toMolecule - projectionToNormal).normalized;

			return Mathf.Acos( Vector3.Dot( Helpers.GetLocalDirection( forwardDirection, centerTransform ), moleculeDirectionInMotorPlane ) ) * Mathf.Rad2Deg;
		}
	}
}
