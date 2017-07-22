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
	}
}