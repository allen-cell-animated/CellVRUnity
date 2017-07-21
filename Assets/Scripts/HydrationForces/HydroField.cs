using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(SphereCollider) )]
public class HydroField : MonoBehaviour 
{
	public float strength = 1f;
	Rigidbody body;

	float _radius = -1f;
	public float radius
	{
		get
		{
			if (_radius < 0)
			{
				_radius = GetComponent<SphereCollider>().radius * transform.lossyScale.x;
			}
			return _radius;
		}
	}

	void Start ()
	{
		body = GetComponentInParent<Rigidbody>();
	}

	void OnTriggerStay (Collider other)
	{
		if (body == null)
		{
			return;
		}

		HydroField otherField = other.GetComponent<HydroField>();
		if (otherField != null)
		{
			OrientToField( otherField );
		}
	}

	void OrientToField (HydroField otherField)
	{
		AddForce( otherField );
		AddTorque( otherField );
	}

	void AddTorque (HydroField otherField)
	{
		float angle = Mathf.Acos( Vector3.Dot( transform.forward, otherField.transform.forward ) );
		Vector3 axis = Vector3.Cross( transform.forward, otherField.transform.forward );
		float speed = strength * 4000f * Mathf.Pow( Vector3.Distance( transform.position, otherField.transform.position ), -2f );
		Vector3 w = angle * speed * axis.normalized;
		Quaternion q = transform.rotation * body.inertiaTensorRotation;
		Vector3 torque = q * Vector3.Scale( body.inertiaTensor, Quaternion.Inverse(q) * w );

		if (VectorIsValid( torque ))
		{
			body.AddTorque( torque );
		}
	}

	void AddForce (HydroField otherField)
	{
		Vector3 toGoalPosition = otherField.transform.TransformPoint( transform.InverseTransformPoint( body.transform.position ) ) - body.transform.position;
		float s = strength * (20000f / (1f * Mathf.Sqrt( 2f * Mathf.PI ))) * Mathf.Exp( -0.5f * Mathf.Pow( toGoalPosition.magnitude - 3.5f, 2f ) / 1f );
		Vector3 force = s * toGoalPosition.normalized;

		if (VectorIsValid( force ))
		{
			body.AddForce( force );
		}
	}

	bool VectorIsValid (Vector3 vector)
	{
		return !float.IsNaN( vector.x ) && !float.IsNaN( vector.y ) && !float.IsNaN( vector.z );
	}
}
