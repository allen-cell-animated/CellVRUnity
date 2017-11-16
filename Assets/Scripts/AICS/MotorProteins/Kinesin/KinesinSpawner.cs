using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MT;

namespace AICS.MotorProteins.Kinesin
{
	public class KinesinSpawner : MonoBehaviour 
	{
		public Kinesin kinesinPrefab;
		public KinesinVesicle vesiclePrefab;
		public Microtubule microtubule;
		public KinesinParameterInput parameterSetter;
		public float number;
		public Vector2 kinesinRange = new Vector2( 0.3f, 0.6f );

		public List<KinesinVesicle> vesicles = new List<KinesinVesicle>();

		void Start ()
		{
			Place( transform, kinesinRange.x, 0 );
//			for (int i = 0; i < number; i++)
//			{
//				Spawn( ((float)i + Random.value) / (float)number );
//			}
			Spawn( 0.35f );
		}

		public void Spawn (float t)
		{
			if (t > kinesinRange.x && t < kinesinRange.y) // spawn as kinesin
			{
				SpawnKinesin( t );
			}
			else // spawn as vesicle
			{
				vesicles.Add( SpawnVesicle( t ) );
			}
		}

		Kinesin SpawnKinesin (float t)
		{
			Kinesin kinesin = Instantiate( kinesinPrefab, transform ) as Kinesin;
			Place( kinesin.transform, t, 14.5f );
			parameterSetter.InitKinesin( kinesin );
			return kinesin;
		}

		KinesinVesicle SpawnVesicle (float t)
		{
			KinesinVesicle vesicle = Instantiate( vesiclePrefab, transform ) as KinesinVesicle;
			Place( vesicle.transform, t, 0 );
			vesicle.t = t;
			vesicle.microtubule = microtubule;
			vesicle.spawner = this;
			return vesicle;
		}

		void Place (Transform obj, float t, float normalOffset)
		{
			Vector3 tangent = microtubule.spline.GetTangent( t );
			Vector3 normal = Quaternion.AngleAxis( Random.Range( 0, 359f ), tangent ) * microtubule.spline.GetNormal( t );
			Vector3 position = microtubule.spline.GetPosition( t ) + normalOffset * normal;
			Quaternion rotation = Quaternion.LookRotation( position + tangent, normal );

			obj.position = position;
			obj.rotation = rotation;
		}

		void Update ()
		{
			foreach (KinesinVesicle vesicle in vesicles)
			{
				if (vesicle.gameObject.activeSelf)
				{
					vesicle.DoUpdate();
				}
			}
		}

		public void ConvertToKinesin (KinesinVesicle vesicle)
		{
			Debug.Log( "convert to kinesin" );
			vesicle.kinesin = SpawnKinesin( vesicle.t );
			vesicle.kinesin.vesicle = vesicle;

			vesicle.gameObject.SetActive( false );
		}

		public void ConvertToVesicle (Kinesin kinesin)
		{
			if (kinesin.vesicle == null)
			{
				Debug.Log( "create vesicle" );
				kinesin.vesicle = SpawnVesicle( kinesinRange.x );
				kinesin.vesicle.kinesin = kinesin;
				vesicles.Add( kinesin.vesicle );
			}
			else
			{
				Debug.Log( "convert to vesicle" );
				Place( kinesin.vesicle.transform, kinesinRange.x, 0 );
				kinesin.vesicle.t = kinesinRange.x;
				kinesin.vesicle.gameObject.SetActive( true );
			}

			kinesin.atpGenerator.DestroyAll();
			Destroy( kinesin.gameObject );
		}

		void OnTriggerEnter (Collider other)
		{
			Hips hips = other.GetComponent<Hips>();
			if (hips != null)
			{
				ConvertToVesicle( hips.kinesin );
			}
		}
	}
}