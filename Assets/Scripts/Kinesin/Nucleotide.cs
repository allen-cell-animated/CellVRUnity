using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Nucleotide : Molecule
	{
		public bool isATP;
		public Color ATPColor;
		public Color ADPColor;

		MeshRenderer _meshRenderer;
		MeshRenderer meshRenderer
		{
			get {
				if (_meshRenderer == null)
				{
					_meshRenderer = GetComponent<MeshRenderer>();
				}
				return _meshRenderer;
			}
		}

		void Start ()
		{
			meshRenderer.material.color = ATPColor;
		}

		public void Hydrolyze ()
		{
			isATP = false;
			meshRenderer.material.color = ADPColor;
		}

		void Update ()
		{
			if (!isBusy && shouldDestroy)
			{
				Destroy( gameObject );
			}
		}
	}
}