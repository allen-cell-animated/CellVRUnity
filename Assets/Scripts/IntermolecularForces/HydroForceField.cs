using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.IntermolecularForces
{
	public class HydroForceField : ForceField 
	{
		public float strength = 1f;

		protected override void InteractWith (Collider other)
		{
			HydroForceField otherField = other.GetComponent<HydroForceField>();
			if (otherField != null)
			{
				OrientToField( otherField.transform, strength );
			}
		}

		protected override Vector3 CalculateAngularAcceleration (Transform otherField)
		{
			float angleForward = Mathf.Acos( Vector3.Dot( transform.forward, otherField.forward ) );
			Vector3 axisForward = Vector3.Cross( transform.forward, otherField.forward );

			return angleForward * axisForward.normalized;
		}
	}
}