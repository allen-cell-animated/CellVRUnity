using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MT;

namespace AICS.MotorProteins.Kinesin
{
	public class MicrotubuleSpawner : MonoBehaviour 
	{
		public GameObject[] prefabs;
		public Microtubule microtubule;
		public int number = 5;
		public float waitTimeBetweenSpawns = 15f;
		public float radialOffset = 14.5f;
		public List<GameObject> spawnedObjects = new List<GameObject>();
		public Transform end;
		public float destroyInterval = 10f;
		public float destroyDistance = 20f;

		bool needToPreWarm = true;
		float lastSpawnTime = -10000f;
		float lastDestroyTime = -10000f;

		void Update ()
		{
			CheckSpawn();
			CheckDestroy();
		}

		void CheckSpawn ()
		{
			if (needToPreWarm || Time.time - lastSpawnTime >= waitTimeBetweenSpawns)
			{
				int i = 0;
				float t = 0;
				while (spawnedObjects.Count < number)
				{
					if (needToPreWarm) 
					{
						t = (float)i / (float)number + Random.value / (float)number;
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
				position = microtubule.spline.GetPosition( t ) + radialOffset * normal;
				rotation = Quaternion.LookRotation( position + tangent, normal );
			}
			IWalkSplines obj = Instantiate( prefabs[Random.Range( 0, prefabs.Length )], position, rotation ).GetComponent<IWalkSplines>();
			SetupObject( obj, t );
		}

		protected virtual void SetupObject (IWalkSplines obj, float t) 
		{
			obj.t = t;
			obj.microtubule = microtubule;
			spawnedObjects.Add( obj.gameObject );
		}

		void CheckDestroy ()
		{
			if (Time.time - lastDestroyTime >= destroyInterval)
			{
				for (int i = 0; i < spawnedObjects.Count; i++)
				{
					if (Vector3.Distance( spawnedObjects[i].transform.position, end.position ) < destroyDistance)
					{
						DoDestruction( spawnedObjects[i] );
						return;
					}
				}
				lastDestroyTime = Time.time;
			}
		}

		void DoDestruction (GameObject obj) 
		{
			if (spawnedObjects.Contains( obj ))
			{
				spawnedObjects.Remove( obj );
			}
			DoCustomDestruction( obj );
			Destroy( obj );
		}

		protected virtual void DoCustomDestruction (GameObject obj) { }
	}
}