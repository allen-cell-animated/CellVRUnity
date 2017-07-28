using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Binding
{
	public enum MoleculeType
	{
		none,
		A,
		B,
		C
	}

	public enum RegulationType
	{
		none,
		activation,
		competitiveInhibition,
		noncompetitiveInhibition,
		uncompetitiveInhibition,
		mixedInhibition
	}

	public class BindingSite : MonoBehaviour 
	{
		public MoleculeType bindingPartner;
		public RegulationType regulationType;
		public HydroForceField hydroField;
		public ElectroForceField electroField;
	}
}