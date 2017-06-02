using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Diffusion
{
	public class ParticleFactory : MonoBehaviour 
	{
		public int n;
		public GameObject particlePrefab;
		public Container container;
		public DiffusingParticle[] particles;
		public float minDisplacement;
		public float maxDisplacement;

		void Start ()
		{
			particles = new DiffusingParticle[n];
			for (int i = 0; i < n; i++)
			{
				particles[i] = Instantiate( particlePrefab, transform.position + container.randomPositionInBounds, 
					Random.rotation ).GetComponent<DiffusingParticle>();
				particles[i].transform.SetParent( transform );
			}
		}

		void Update ()
		{
			foreach (DiffusingParticle particle in particles)
			{
				SetDisplacementBounds( particle.displacement );
				particle.SetDisplacementColor();
			}
		}

		void SetDisplacementBounds (float displacement)
		{
			if (displacement < minDisplacement)
			{
				minDisplacement = displacement;
			}
			if (displacement > maxDisplacement)
			{
				maxDisplacement = displacement;
			}
		}
	}
}