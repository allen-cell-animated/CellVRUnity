using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.IntermolecularForces
{
	[RequireComponent( typeof(SphereCollider) )]
	public abstract class ForceField : MonoBehaviour 
	{
		protected Rigidbody body;

		void Start ()
		{
			body = GetComponentInParent<Rigidbody>();
		}

		void OnTriggerStay (Collider other)
		{
			if (body != null)
			{
				InteractWith( other );
			}
		}

		protected abstract void InteractWith (Collider other);

		protected void OrientToField (Transform otherField, float strength)
		{
			AddForce( otherField, strength );
			AddTorque( otherField, strength );
		}

		protected void AddTorque (Transform otherField, float strength)
		{
			float angle = Mathf.Acos( Vector3.Dot( transform.forward, otherField.forward ) );
			Vector3 axis = Vector3.Cross( transform.forward, otherField.forward );
			float speed = strength * 8000f * Mathf.Pow( Vector3.Distance( transform.position, otherField.position ), -2f );
			Vector3 w = angle * speed * axis.normalized;
			Quaternion q = transform.rotation * body.inertiaTensorRotation;
			Vector3 torque = q * Vector3.Scale( body.inertiaTensor, Quaternion.Inverse(q) * w );

			if (VectorIsValid( torque ))
			{
				body.AddTorque( torque );
			}
		}

		protected void AddForce (Transform otherField, float strength)
		{
			Vector3 toGoalPosition = otherField.TransformPoint( transform.InverseTransformPoint( body.transform.position ) ) - body.transform.position;
			float s = strength * (40000f / (1f * Mathf.Sqrt( 2f * Mathf.PI ))) * Mathf.Exp( -0.5f * Mathf.Pow( toGoalPosition.magnitude - 3.5f, 2f ) / 1f );
			Vector3 force = s * toGoalPosition.normalized;

			if (VectorIsValid( force ))
			{
				body.AddForce( force );
			}
		}

		protected bool VectorIsValid (Vector3 vector)
		{
			return !float.IsNaN( vector.x ) && !float.IsNaN( vector.y ) && !float.IsNaN( vector.z );
		}
	}
}