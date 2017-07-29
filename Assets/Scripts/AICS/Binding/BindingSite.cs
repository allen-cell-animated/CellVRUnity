using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Binding
{
	public enum MoleculeType
	{
		none,
		KinesinMotor,
		Tubulin,
		ATP,
		ADP
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

		public Color hydroColor;
		public Color electroColor;
		Color startColor;

		MeshRenderer _meshRenderer;
		MeshRenderer meshRenderer
		{
			get
			{
				if (_meshRenderer == null)
				{
					_meshRenderer = transform.parent.GetComponentInChildren<MeshRenderer>();
				}
				return _meshRenderer;
			}
		}

		void Start ()
		{
			startColor = meshRenderer.material.color;
		}

		void Update ()
		{
			if (electroField.interacting)
			{
				meshRenderer.material.color = electroColor;
			}
			else if (hydroField.interacting)
			{
				meshRenderer.material.color = hydroColor;
			}
			else
			{
				meshRenderer.material.color = startColor;
			}
		}
	}
}