using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Binding
{
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
				AddForce( otherField, strength );
				AddTorque( otherField, angularStrength );
			}
			else
			{
				interacting = false;
			}
		}

		protected override Vector3 CalculateAngularAccelerationDirection (Transform otherField)
		{
			float angleForward = Mathf.Acos( Vector3.Dot( transform.forward, otherField.forward ) );
			Vector3 axisForward = Vector3.Cross( transform.forward, otherField.forward );

			float angleUp = Mathf.Acos( Vector3.Dot( transform.up, otherField.up ) );
			Vector3 axisUp = Vector3.Cross( transform.up, otherField.up );

			return angleForward * axisForward.normalized + angleUp * axisUp.normalized;
		}
	}
}