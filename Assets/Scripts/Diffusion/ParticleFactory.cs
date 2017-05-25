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

		static ParticleFactory _Instance;
		public static ParticleFactory Instance
		{
			get {
				if (_Instance == null)
				{
					_Instance = GameObject.FindObjectOfType<ParticleFactory>();
				}
				return _Instance;
			}
		}

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
	}
}