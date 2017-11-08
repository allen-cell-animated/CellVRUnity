using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MT;

namespace AICS.MotorProteins.Kinesin
{
	public class KinesinSpawner : MonoBehaviour 
	{
		public Kinesin kinesinPrefab;
		public bool spawn = true;
		public int numberOfKinesins = 5;
		public float waitTimeBetweenSpawns = 15f;
		public KinesinParameterInput parameterSetter;
		public Microtubule microtubule; 

		bool needToPreWarm = true;
		List<Kinesin> kinesins = new List<Kinesin>();
		float lastSpawnTime = -100f;

		void Update ()
		{
			if (spawn && (needToPreWarm || Time.time - lastSpawnTime >= waitTimeBetweenSpawns))
			{
				int i = 0;
				float t = 0;
				while (kinesins.Count < numberOfKinesins)
				{
					if (needToPreWarm) 
					{
						t = (float)i / (float)numberOfKinesins + Random.value / (float)numberOfKinesins;
						i++;
					}
					Spawn( t );
				}
				lastSpawnTime = Time.time;
				needToPreWarm = false;
			}
		}

		public void Spawn (float t)
		{
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;
			if (needToPreWarm)
			{
				Vector3 tangent = microtubule.spline.GetTangent( t );
				Vector3 normal = Quaternion.AngleAxis( Random.Range( 0, 359f ), tangent ) * microtubule.spline.GetNormal( t );
				position = microtubule.spline.GetPosition( t ) + (microtubule.radius + 7f) * normal;
				rotation = Quaternion.LookRotation( position + tangent, normal );
			}
			Kinesin kinesin = Instantiate( kinesinPrefab, position, rotation ) as Kinesin;
			parameterSetter.InitKinesin( kinesin );
			kinesins.Add( kinesin );
		}

		public void KinesinWasDestroyed (Kinesin kinesin)
		{
			if (kinesins.Contains( kinesin ))
			{
				kinesins.Remove( kinesin );
			}
		}
	}
}