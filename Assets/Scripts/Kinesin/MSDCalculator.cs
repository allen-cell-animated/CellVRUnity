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
		string data = "time,msd\n";
		float lastDataTime = -1000000f;
		bool startedLogging;

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
			foreach (RandomForces particle in particles)
			{
				particle.SetStartPosition();
			}
			startedLogging = true;
		}

		void CalculateMSD ()
		{
			float sum = 0;
			foreach (RandomForces particle in particles)
			{
				particle.CalculateDisplacement();
				sum += Mathf.Pow( particle.displacement, 2f );
			}
			meanSquaredDisplacement = sum / n;
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

		void OnDrawGizmos ()
		{
			Gizmos.DrawWireCube( transform.position, size );
		}

		void OnApplicationQuit ()
		{
			Debug.Log( data );
		}
	}
}