using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class CollisionAvoider : MolecularComponent, IValidateMoves
	{
		public bool exitCollisions = true;

		MoleculeDetector detector;

		Rigidbody _body;
		Rigidbody body
		{
			get
			{
				if (_body == null)
				{
					_body = GetComponent<Rigidbody>();
					_body.useGravity = false;
					_body.isKinematic = true;
				}
				return _body;
			}
		}

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