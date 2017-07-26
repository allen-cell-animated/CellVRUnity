using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.IntermolecularForces
{
	public enum MoleculeType
	{
		A,
		B,
		C
	}

	public class ElectroForceField : ForceField 
	{
		public MoleculeType type;
		public List<MoleculeType> bindingPartners;
		public float strength = 500f;

		protected override void InteractWith (Collider other)
		{
			ElectroForceField otherField = other.GetComponent<ElectroForceField>();
			if (otherField != null && bindingPartners.Contains( otherField.type ))
			{
				OrientToField( otherField.transform, strength );
			}
		}

		protected override Vector3 CalculateAngularAcceleration (Transform otherField)
		{
			float angleForward = Mathf.Acos( Vector3.Dot( transform.forward, otherField.forward ) );
			Vector3 axisForward = Vector3.Cross( transform.forward, otherField.forward );

			float angleUp = Mathf.Acos( Vector3.Dot( transform.up, otherField.up ) );
			Vector3 axisUp = Vector3.Cross( transform.up, otherField.up );

			return angleForward * axisForward.normalized + angleUp * axisUp.normalized;
		}
	}
}