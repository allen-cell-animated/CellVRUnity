using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class DynamicColorer : MolecularComponent
	{
		public MeshRenderer meshRenderer;
		public Color dynamicColor;

		Color startColor;

		void Start ()
		{
			startColor = meshRenderer.material.color;
		}

		public void SetColor ()
		{
			meshRenderer.material.color = dynamicColor;
		}

		public void ResetColor ()
		{
			meshRenderer.material.color = startColor;
		}
	}
}