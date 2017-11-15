using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MT;

namespace AICS.MotorProteins.Kinesin
{
	public class VesicleSpawner : MonoBehaviour 
	{
		public Vesicle[] prefabs;
		public Microtubule microtubule;
		public int number = 5;
		public float radialOffset = 14.5f;
		public List<Vesicle> vesicles = new List<Vesicle>();

		void Start ()
		{
			for (int i = 0; i < number; i++)
			{
				Spawn( ((float)i + Random.value) / (float)number );
			}
		}

		void Update ()
		{
			foreach (Vesicle vesicle in vesicles)
			{
				vesicle.DoUpdate();
			}
		}

		void Spawn (float t)
		{
			Vector3 tangent = microtubule.spline.GetTangent( t );
			Vector3 normal = Quaternion.AngleAxis( Random.Range( 0, 359f ), tangent ) * microtubule.spline.GetNormal( t );
			Vector3 position = microtubule.spline.GetPosition( t ) + radialOffset * normal;
			Quaternion rotation = Quaternion.LookRotation( position + tangent, normal );

			Vesicle vesicle = Instantiate( prefabs[Random.Range( 0, prefabs.Length )], position, rotation ) as Vesicle;
			vesicle.transform.SetParent( transform );
			vesicle.t = t;
			vesicle.microtubule = microtubule;
			vesicles.Add( vesicle );
		}
	}
}