using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.PhysicsKinesin
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

		Collider _collider;
		Collider theCollider
		{
			get {
				if (_collider == null)
				{
					_collider = GetComponent<Collider>();
				}
				return _collider;
			}
		}

		RandomForces _randomForces;
		RandomForces randomForces
		{
			get {
				if (_randomForces == null)
				{
					_randomForces = GetComponent<RandomForces>();
				}
				return _randomForces;
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

		protected override void Hide ()
		{
			meshRenderer.enabled = theCollider.enabled = randomForces.enabled = false;
		}

		protected override void Show ()
		{
			meshRenderer.enabled = theCollider.enabled = randomForces.enabled = true;
		}

		protected override void Reset ()
		{
			isATP = true;
			meshRenderer.material.color = ATPColor;
		}
	}
}