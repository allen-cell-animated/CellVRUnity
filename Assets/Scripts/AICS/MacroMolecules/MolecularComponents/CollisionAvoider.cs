using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class CollisionAvoider : MolecularComponent, IValidateMoves, IFind
	{
		public bool exitCollisions = true;

		public BindingCriteria criteriaToFind 
		{
			get
			{
				return new BindingCriteria( MoleculeType.All, 0 );
			}
		}

		public float searchRadius 
		{ 
			get
			{
				return molecule.radius;
			}
		}

		public bool MoveIsValid (Vector3 position, float radius)
		{
			if (molecule.detector == null) { Debug.Log( molecule.name ); }
			return !molecule.detector.WillCollide( position, radius );
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