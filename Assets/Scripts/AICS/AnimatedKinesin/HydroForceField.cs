using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AnimatedKinesin
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
			if (otherField != null && otherField.surface != null)
			{
				float fieldAngle = Mathf.Acos( Mathf.Clamp( Vector3.Dot( transform.forward, other.transform.forward ), -1f, 1f ) );

				float surfaceToOtherSurfaceDistance = Vector3.Distance( otherField.surface.position, surface.transform.position );
				float surfaceToOtherFieldDistance = Vector3.Distance( otherField.transform.position, surface.transform.position );

				Vector3 surfaceToOtherSurface = Vector3.Normalize( otherField.surface.position - surface.position );
				Vector3 surfaceToField = Vector3.Normalize( transform.position - surface.transform.position );
				float surfaceAngle = Mathf.Acos( Vector3.Dot( surfaceToOtherSurface, surfaceToField ) );

				if (fieldAngle < Mathf.PI / 3f && surfaceToOtherSurfaceDistance > surfaceToOtherFieldDistance && surfaceAngle < Mathf.PI / 3f)
				{
					interacting = true;
					Move( otherField, strength );
					Rotate( otherField, angularStrength );
				}
				else 
				{
					interacting = false;
				}
			}
			else 
			{
				interacting = false;
			}
		}

		protected override Vector3 CalculateRotation (Transform otherField, float speed)
		{
			float angleForward = 180f * Mathf.Acos( Vector3.Dot( transform.forward, otherField.forward ) ) / Mathf.PI;
			Vector3 axisForward = Vector3.Cross( transform.forward, otherField.forward );

			return Quaternion.AngleAxis( speed * angleForward, axisForward ).eulerAngles;
		}
	}
}