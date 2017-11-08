using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins.Kinesin
{
	public class KinesinSpawner : MonoBehaviour 
	{
		public Kinesin kinesinPrefab;
		public int numberOfKinesins = 5;
		public float waitTimeBetweenSpawns = 15f;
		public KinesinParameterInput parameterSetter;

		List<Kinesin> kinesins = new List<Kinesin>();
		float lastSpawnTime = -100f;

		void Update ()
		{
			if (kinesins.Count < numberOfKinesins && Time.time - lastSpawnTime >= waitTimeBetweenSpawns)
			{
				Spawn();
				lastSpawnTime = Time.time;
			}
		}

		public void Spawn ()
		{
			Kinesin kinesin = Instantiate( kinesinPrefab, transform.position, transform.rotation ) as Kinesin;
			parameterSetter.InitKinesin( kinesin );
			kinesins.Add( kinesin );
		}

		public void KinesinWasDestroyed (Kinesin kinesin)
		{
			if (kinesins.Contains( kinesin ))
			{
				kinesins.Remove( kinesin );
				Spawn();
			}
		}
	}
}