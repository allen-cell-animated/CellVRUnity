using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Diffusion
{
	[RequireComponent( typeof(Collider) )]
	public class Boundary : MonoBehaviour 
	{
		Container _container;
		Container container
		{
			get {
				if (_container == null)
				{
					_container = GetComponentInParent<Container>();
				}
				return _container;
			}
		}

		void Start ()
		{
			Collider collider = GetComponent<Collider>();
			collider.isTrigger = true;
		}

		void OnTriggerEnter (Collider other)
		{
			DiffusingParticle particle = other.GetComponent<DiffusingParticle>();
			if (particle != null)
			{
				container.ParticleCollided( particle );
			}
		}
	}
}