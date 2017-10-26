using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public class ParticleMetaballs : MonoBehaviour 
	{
		public Transform[] points;
		public ParticleSystem emitter;

		ParticleSystem.Particle[] particles;

		void Start () 
		{
			EmitParticlePerPoint();
		}

		void EmitParticlePerPoint ()
		{
			ParticleSystem.EmitParams parameters = new ParticleSystem.EmitParams();
			parameters.velocity = Vector3.zero;
			parameters.startSize = 1f;
			parameters.startLifetime = Mathf.Infinity;
			parameters.startColor = Color.white;

			for (int i = 0; i < points.Length; i++)
			{
				parameters.randomSeed = (uint)i;
				parameters.position = points[i].position;
				emitter.Emit( parameters, 1 );
			}

			particles = new ParticleSystem.Particle[points.Length];
		}

		void Update () 
		{
			UpdateParticlePositions();
		}

		void UpdateParticlePositions ()
		{
			emitter.GetParticles( particles );
			for (int i = 0; i < particles.Length; i++)
			{
				particles[i].position = points[particles[i].randomSeed].position;
			}
			emitter.SetParticles( particles, particles.Length );
		}
	}
}