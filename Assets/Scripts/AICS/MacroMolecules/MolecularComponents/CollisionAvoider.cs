using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class CollisionAvoider : MolecularComponent, IValidateMoves
	{
		public bool exitCollisions = true;

		MoleculeDetector detector;

		public bool MoveIsValid (Vector3 position, float radius)
		{
			return !detector.WillCollide( position, radius );
		}

		void Awake ()
		{
			CreateDetector();
		}

		void CreateDetector ()
		{
			detector = (Instantiate( Resources.Load( "Prefabs/MoleculeDetector" ), transform ) as GameObject).GetComponent<MoleculeDetector>().Setup( MoleculeType.All, molecule.radius );
			detector.name = "Collider";
		}

		void OnTriggerStay (Collider other)
		{
			if (exitCollisions)
			{
				ExitCollision( other.transform.position );
			}
		}

		void ExitCollision (Vector3 otherPosition)
		{
			molecule.MoveIfValid( 0.1f * (transform.position - otherPosition) );
		}
	}
}