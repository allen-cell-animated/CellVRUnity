using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins
{
	[System.Serializable]
	public class MoleculeGraph<T> where T : Molecule
	{
		Direction forwardDirection;
		Direction rightDirection;
		Dictionary<Molecule,float> moleculeAngles = new Dictionary<Molecule,float>();
		Transform lastCenterTransform;

		public MoleculeGraph (Direction _forwardDirection, Direction _rightDirection)
		{
			forwardDirection = _forwardDirection;
			rightDirection = _rightDirection;
		}

		public void AddMolecules (List<T> molecules, Transform centerTransform)
		{
			moleculeAngles.Clear();
			lastCenterTransform = centerTransform;
			Molecule molecule;
			float angle;
			Debug.Log( centerTransform.name + " graph tubulins-------------------------------------" );
			foreach (T m in molecules)
			{
				molecule = m as Molecule;
				angle = GetMoleculeAngleFromForward( molecule );
				moleculeAngles.Add( molecule, angle );
				Debug.Log( molecule.name + " = " + angle );
			}
		}

		// Get angle in degrees starting at 12 o'clock (forward) and going clockwise
		float GetMoleculeAngleFromForward (Molecule molecule)
		{
			Vector3 toMolecule = Vector3.Normalize( molecule.transform.position - lastCenterTransform.position );
			float angleForward = Mathf.Acos( Vector3.Dot( Helpers.GetLocalDirection( forwardDirection, lastCenterTransform ), toMolecule ) ) * Mathf.Rad2Deg;
			float angleRight = Mathf.Acos( Vector3.Dot( Helpers.GetLocalDirection( rightDirection, lastCenterTransform ), toMolecule ) ) * Mathf.Rad2Deg;

			if (angleRight <= 90f)
			{
				return Mathf.Round( angleForward );
			}
			else
			{
				return Mathf.Round( 360f - angleForward );
			}
		}
	}
}