using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Diffusion
{
	public class MSDCalculator : MonoBehaviour 
	{
		public float meanSquaredDisplacement;

		string data = "time,msd\n";
		float lastDataTime = -1000000f;
		bool startedLogging;

		DiffusingParticle[] _particles;
		DiffusingParticle[] particles
		{
			get {
				if (_particles == null)
				{
					_particles = ParticleFactory.Instance.particles;
				}
				return _particles;
			}
		}

		void Update ()
		{
			if (Time.time > 1f)
			{
				if (!startedLogging)
				{
					StartLogging();
				}

				CalculateMSD();

				LogMSD();
			}
		}

		void StartLogging ()
		{
			ParameterInput.Instance.simulationTimePassed = 0;
			foreach (DiffusingParticle particle in particles)
			{
				particle.SetStartPosition();
			}
			startedLogging = true;
		}

		void CalculateMSD ()
		{
			float sum = 0;
			foreach (DiffusingParticle particle in particles)
			{
				sum += Mathf.Pow( particle.displacement, 2f );
			}
			meanSquaredDisplacement = sum / particles.Length;
		}

		void LogMSD ()
		{
			float t = ParameterInput.Instance.simulationTimePassed;
			if (t - lastDataTime >= ParameterInput.Instance.dTime.value)
			{
				data += t + "," + meanSquaredDisplacement + "\n";
				lastDataTime = t;
			}
		}

		void OnApplicationQuit ()
		{
			Debug.Log( data );
		}
	}
}