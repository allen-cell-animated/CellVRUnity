using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class MSDCalculator : MonoBehaviour 
	{
		public int n;
		public GameObject particlePrefab;
		public Vector3 size;
		public float meanSquaredDisplacement;

		RandomForces[] particles;
		float lastTime = -1f;

		void Start ()
		{
			particles = new RandomForces[n];
			for (int i = 0; i < n; i++)
			{
				particles[i] = Instantiate( particlePrefab, transform.position + randomPositionInBounds, Random.rotation ).GetComponent<RandomForces>();
				particles[i].transform.SetParent( transform );
			}
		}

		Vector3 randomPositionInBounds
		{
			get {
				return new Vector3( Random.Range( -size.x / 2f, size.x / 2f ),
					Random.Range( -size.y / 2f, size.y / 2f ),
					Random.Range( -size.z / 2f, size.z / 2f ) );
			}
		}

		void Update ()
		{
			if (Time.time - lastTime > 1f)
			{
				float sum = 0;
				foreach (RandomForces particle in particles)
				{
					particle.CalculateDisplacement();
					sum += Mathf.Pow( particle.displacement, 2f );
				}
				meanSquaredDisplacement = sum / n;

				lastTime = Time.time;
			}
		}

		void OnDrawGizmos ()
		{
			Gizmos.DrawWireCube( transform.position, size );
		}
	}
}