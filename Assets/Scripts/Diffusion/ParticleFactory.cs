using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.IO;

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
		[Tooltip( "particles per clock second")]
		public float escapeRate;

		float lastTime;
		float numberEscaped;

//		string data = "mean,sample\n";

		void Start ()
		{
			particles = new DiffusingParticle[n];
			for (int i = 0; i < n; i++)
			{
				particles[i] = Instantiate( particlePrefab, container.randomPositionInBounds, Random.rotation ).GetComponent<DiffusingParticle>();
				particles[i].transform.SetParent( transform );
			}
		}

		void Update ()
		{
			foreach (DiffusingParticle particle in particles)
			{
				KeepInBounds( particle.transform );
				SetDisplacementBounds( particle.displacement );
				particle.SetDisplacementColor();
			}

			if (Time.time - lastTime > 0.5f)
			{
				escapeRate = numberEscaped / (Time.time - lastTime);
				numberEscaped = 0;
				lastTime = Time.time;
			}
		}

		void KeepInBounds (Transform particle)
		{
			if (!container.PositionIsInBounds( particle.position ))
			{
				particle.position = container.randomPositionInBounds;
				numberEscaped++;
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

//		public void RecordData (float meanForce, float force)
//		{
//			data += meanForce + "," + force + "\n";
//		}
//
//		void OnApplicationQuit ()
//		{
//			File.WriteAllText( "/Users/blairl/Dropbox/AICS/MotorProteins/cytoplasmSimulations/MSDData/Auto/forces.csv", data );
//		}
	}
}