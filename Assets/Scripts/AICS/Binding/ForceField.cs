using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Binding
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

		protected void AddTorque (ForceField otherField, float strength)
		{
			float acceleration = strength * 4e3f * Mathf.Pow( Vector3.Distance( transform.position, otherField.transform.position ), -2f );
			Vector3 angularAcceleration = acceleration * CalculateAngularAccelerationDirection( otherField.transform );

			Quaternion q = transform.rotation * body.inertiaTensorRotation;
			Vector3 torque = q * Vector3.Scale( body.inertiaTensor, Quaternion.Inverse(q) * angularAcceleration );

			if (VectorIsValid( torque ))
			{
				body.AddTorque( torque );
			}
		}

		protected abstract Vector3 CalculateAngularAccelerationDirection (Transform otherField);

		protected void AddForce (ForceField otherField, float strength)
		{
			Vector3 toGoalPosition = otherField.transform.TransformPoint( transform.InverseTransformPoint( body.transform.position ) ) - body.transform.position;
			float s = strength * (2e4f / (1f * Mathf.Sqrt( 2f * Mathf.PI ))) * Mathf.Exp( -0.5f * Mathf.Pow( toGoalPosition.magnitude - 3.5f, 2f ) / 1f );
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