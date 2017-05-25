using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace AICS.Diffusion
{
	public class MSDCalculator : MonoBehaviour 
	{
		public string filePath = "/Users/blairl/Dropbox/AICS/MotorProteins/cytoplasmSimulations/MSDData/Auto/";
		public float simulatedMSD;
		public float theoreticalMSD;
		public float simulationTimePassed;

		string data = "time,theoretical msd,simulated msd\n";
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

				CalculateSimulatedMSD();
				CalculateTheoreticalMSD();
				LogMSD();

				simulationTimePassed += ParameterInput.Instance.dTime.value;
			}
		}

		void StartLogging ()
		{
			foreach (DiffusingParticle particle in particles)
			{
				particle.SetStartPosition();
			}
			startedLogging = true;
		}

		void CalculateSimulatedMSD ()
		{
			float sum = 0;
			foreach (DiffusingParticle particle in particles)
			{
				sum += Mathf.Pow( particle.displacement, 2f );
			}
			simulatedMSD = sum / particles.Length;
		}

		void CalculateTheoreticalMSD ()
		{
			theoreticalMSD = 6f * 0.01f * ParameterInput.Instance.diffusionCoefficient.value * simulationTimePassed;
		}

		void LogMSD ()
		{
			float t = simulationTimePassed;
			if (t - lastDataTime >= ParameterInput.Instance.dTime.value)
			{
				data += t + "," + theoreticalMSD + "," + simulatedMSD + "\n";
				lastDataTime = t;
			}
		}

		void OnApplicationQuit ()
		{
			string fileName = "msd" 
				+ "_vm" + ParameterInput.Instance.velocityMultiplier + "sqrt"
				+ "_n" + particles.Length 
				+ "_t" + ParameterInput.Instance.dTime.value 
				+ "_dc" + ParameterInput.Instance.diffusionCoefficient.value.ToString().Split('.')[1]
				+ "_drag" + particles[0].GetComponent<Rigidbody>().drag.ToString().Split('.')[0] + ".csv";
			File.WriteAllText( filePath + fileName, data );
		}
	}
}