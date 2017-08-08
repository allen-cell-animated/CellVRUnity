using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AICS.AnimatedKinesin
{
	[RequireComponent( typeof(Collider) )]
	public abstract class ForceField : MonoBehaviour 
	{
		public bool interacting;
		public Transform surface;

		protected Molecule molecule;

		void Start ()
		{
			if (surface != null)
			{
				molecule = surface.GetComponent<Molecule>();
			}
		}

		void OnTriggerStay (Collider other)
		{
			if (molecule != null)
			{
				InteractWith( other );
			}
		}

		protected abstract void InteractWith (Collider other);

		protected void Move (ForceField otherField, float strength)
		{
			Vector3 toGoalPosition = otherField.transform.TransformPoint( transform.InverseTransformPoint( molecule.transform.position ) ) - molecule.transform.position;
			Debug.Log( "position = " + toGoalPosition.magnitude );
//			if (toGoalPosition.magnitude > 0.01f)
//			{
				float s = (strength / (1f * Mathf.Sqrt( 2f * Mathf.PI ))) * Mathf.Exp( -0.5f * Mathf.Pow( toGoalPosition.magnitude - 2.5f, 2f ) / 1f );
				Vector3 moveStep = s * toGoalPosition.normalized;

//				molecule.MoveIfValid( moveStep );
				molecule.transform.position += moveStep;
//			}
		}

		protected void Rotate (ForceField otherField, float strength)
		{
			float speed = Mathf.Min( strength * 0.01f * Mathf.Pow( Vector3.Distance( transform.position, otherField.transform.position ), -2f ), 1f );
			Vector3 rotationStep = CalculateRotation( otherField.transform, speed );
			Debug.Log( "rotation = " + rotationStep.magnitude );
			if (rotationStep.magnitude > 0)
			{
				molecule.transform.Rotate( rotationStep, Space.World );
			}
		}

		protected abstract Vector3 CalculateRotation (Transform otherField, float speed);

		protected bool VectorIsValid (Vector3 vector)
		{
			return !float.IsNaN( vector.x ) && !float.IsNaN( vector.y ) && !float.IsNaN( vector.z );
		}

		void OnTriggerExit (Collider other)
		{
			interacting = false;
		}
	}
}