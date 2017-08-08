using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AnimatedKinesin
{
	public enum MoleculeType
	{
		none,
		KinesinMotor,
		Tubulin,
		ATP,
		ADP
	}

	// Simulate electrostatic and van der Waals (electrodynamic) forces 
	// - only at distances < 0.6 nm
	// - 0.02 pN force per atom pair
	// - specific binding orientation for specific binding partner
	public class ElectroForceField : ForceField 
	{
		public MoleculeType type;
		public List<MoleculeType> bindingPartners;
		public float strength = 100f;
		public float angularStrength = 500f;

		protected override void InteractWith (Collider other)
		{
			ElectroForceField otherField = other.GetComponent<ElectroForceField>();
			if (otherField != null && bindingPartners.Contains( otherField.type ))
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

		protected override Vector3 CalculateRotation (Transform otherField, float speed)
		{
			float angleForward = 180f * Mathf.Acos( Mathf.Clamp( Vector3.Dot( transform.forward, otherField.forward ), -1f, 1f ) ) / Mathf.PI;
			Vector3 axisForward = Vector3.Cross( transform.forward, otherField.forward );

			float angleUp = 180f * Mathf.Acos( Mathf.Clamp( Vector3.Dot( transform.up, otherField.up ), -1f, 1f ) ) / Mathf.PI;
			Vector3 axisUp = Vector3.Cross( transform.up, otherField.up );

			return (Quaternion.AngleAxis( speed * angleForward, axisForward ) * Quaternion.AngleAxis( speed * angleUp, axisUp )).eulerAngles;
		}
	}
}