using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Diffusion
{
	public class Container : MonoBehaviour 
	{
		public Vector3 size;
		
		public Vector3 randomPositionInBounds
		{
			get {
				return new Vector3( Random.Range( -size.x / 2f, size.x / 2f ),
					Random.Range( -size.y / 2f, size.y / 2f ),
					Random.Range( -size.z / 2f, size.z / 2f ) );
			}
		}

		public void ParticleCollided (DiffusingParticle particle)
		{
			Vector3 particleToCenter = Vector3.Normalize( transform.position - particle.transform.position );
			RaycastHit hit;
			if (Physics.Raycast( transform.position, particleToCenter, out hit, Mathf.Infinity, 1 << gameObject.layer ))
			{
				particle.transform.position = transform.position 
					+ (hit.distance - particle.transform.localScale.x) * particleToCenter;
			}
		}

		void OnDrawGizmos ()
		{
			Gizmos.DrawWireCube( transform.position, size );
		}
	}
}