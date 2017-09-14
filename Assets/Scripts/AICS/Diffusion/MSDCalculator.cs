using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace AICS.Diffusion
{
	[RequireComponent( typeof(ParticleFactory) )]
	public class MSDCalculator : MonoBehaviour 
	{
		public bool saveMSDDataToFile;
		public bool saveDisplacementDataToFile;
		public string filePath = "/Users/blairl/Dropbox/AICS/MotorProteins/cytoplasmSimulations/MSDData/Auto/";
		public float simulatedMSD;
		public float theoreticalMSD;
		public float simulationTimePassed;

		string MSDdata = "time,theoretical msd,simulated msd\n";
		string displacementData = "";
		float lastDataTime = -1000000f;
		bool startedLogging;

		ParticleFactory _particleFactory;
		ParticleFactory particleFactory
		{
			get {
				if (_particleFactory == null)
				{
					_particleFactory = GetComponent<ParticleFactory>();
				}
				return _particleFactory;
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

				simulationTimePassed += DiffusionParameterInput.Instance.dTime.value;
			}
		}

		void StartLogging ()
		{
			foreach (DiffusingParticle particle in particleFactory.particles)
			{
				particle.SetStartPosition();
			}
			startedLogging = true;
		}

		void CalculateSimulatedMSD ()
		{
			float sum = 0;
			foreach (DiffusingParticle particle in particleFactory.particles)
			{
				sum += Mathf.Pow( particle.displacement, 2f );
			}
			simulatedMSD = sum / particleFactory.particles.Length;
		}

		void CalculateTheoreticalMSD ()
		{
			theoreticalMSD = 6f * 0.01f * DiffusionParameterInput.Instance.diffusionCoefficient.value * 1E-4f * simulationTimePassed;
		}

		void LogMSD ()
		{
			float t = simulationTimePassed;
			if (t - lastDataTime >= DiffusionParameterInput.Instance.dTime.value)
			{
				MSDdata += t + "," + theoreticalMSD + "," + simulatedMSD + "\n";
				lastDataTime = t;
			}
		}

		public void LogDisplacement (float displacement)
		{
			displacementData += displacement + "\n";
		}

		void OnApplicationQuit ()
		{
			if (saveMSDDataToFile)
			{
				string fileName = "msd" 
					+ "_vm" + DiffusionParameterInput.Instance.forceMultiplier + "sqrt"
					+ "_n" + particleFactory.particles.Length 
					+ "_t" + DiffusionParameterInput.Instance.dTime.value 
					+ "_dc" + DiffusionParameterInput.Instance.diffusionCoefficient.value.ToString().Split('.')[0]
					+ "_drag" + particleFactory.particles[0].GetComponent<Rigidbody>().drag.ToString().Split('.')[0] + ".csv";
				File.WriteAllText( filePath + fileName, MSDdata );
			}
			if (saveDisplacementDataToFile)
			{
				string fileName = "displacement.csv";
				File.WriteAllText( filePath + fileName, displacementData );
			}
		}
	}
}