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
	}
}