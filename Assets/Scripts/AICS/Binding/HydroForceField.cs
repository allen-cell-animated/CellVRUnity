using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Binding
{
	// Simulate interaction of two charged surfaces over a field of water molecules
	// - at distances < around 3 nm
	// - 40(?) pN force per surface pair
	// - orient on one axis nonspecifically
	public class HydroForceField : ForceField 
	{
		public float strength = 1f;
		public float angularStrength = 1f;

		protected override void InteractWith (Collider other)
		{
			HydroForceField otherField = other.GetComponent<HydroForceField>();
			if (otherField != null)
			{
				AddForce( otherField, strength );
				AddTorque( otherField, angularStrength );
			}
		}

		protected override Vector3 CalculateAngularAccelerationDirection (Transform otherField)
		{
			float angleForward = Mathf.Acos( Vector3.Dot( transform.forward, otherField.forward ) );
			Vector3 axisForward = Vector3.Cross( transform.forward, otherField.forward );

			return angleForward * axisForward.normalized;
		}
	}
}