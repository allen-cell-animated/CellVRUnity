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
			Place( transform, kinesinRange.x, 0, 0 );
			for (int i = 0; i < number; i++)
			{
				Spawn( ((float)i + Random.value) / (float)number );
			}
		}

		public void Spawn (float t)
		{
			if (t > kinesinRange.x && t < kinesinRange.y) // spawn as kinesin
			{
				SpawnKinesin( t, 0 );//Random.Range( 15f, 165f ) );
			}
			else // spawn as vesicle
			{
				vesicles.Add( SpawnVesicle( t, 0 ) );//Random.Range( 15f, 165f ) ) );
			}
		}

		Kinesin SpawnKinesin (float t, float normalRotation)
		{
			Kinesin kinesin = Instantiate( kinesinPrefab, transform ) as Kinesin;
			Place( kinesin.transform, t, 14.5f, normalRotation );
			parameterSetter.InitKinesin( kinesin );
			return kinesin;
		}

		KinesinVesicle SpawnVesicle (float t, float normalRotation)
		{
			KinesinVesicle vesicle = Instantiate( vesiclePrefab, transform ) as KinesinVesicle;
			Place( vesicle.transform, t, 0, normalRotation );
			vesicle.t = t;
			vesicle.normalRotation = normalRotation;
			vesicle.microtubule = microtubule;
			vesicle.spawner = this;
			return vesicle;
		}

		void Place (Transform obj, float t, float normalOffset, float normalRotation)
		{
			Vector3 tangent = microtubule.spline.GetTangent( t );
			Vector3 normal = Quaternion.AngleAxis( normalRotation, tangent ) * microtubule.spline.GetNormal( t );
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
			vesicle.kinesin = SpawnKinesin( vesicle.t, vesicle.normalRotation );
			vesicle.kinesin.vesicle = vesicle;

			vesicle.gameObject.SetActive( false );
		}

		public void ConvertToVesicle (Kinesin kinesin)
		{
			if (kinesin.vesicle == null)
			{
				kinesin.vesicle = SpawnVesicle( kinesinRange.x, GetKinesinNormalRotation( kinesin ) );
				kinesin.vesicle.kinesin = kinesin;
				kinesin.vesicle.sprite.position = kinesin.cargo.transform.position;
				kinesin.vesicle.sprite.rotation = kinesin.cargo.transform.rotation;
				vesicles.Add( kinesin.vesicle );
			}
			else
			{
				Place( kinesin.vesicle.transform, kinesinRange.x, 0, GetKinesinNormalRotation( kinesin ) );
				kinesin.vesicle.t = kinesinRange.x;
				kinesin.vesicle.sprite.position = kinesin.cargo.transform.position;
				kinesin.vesicle.sprite.rotation = kinesin.cargo.transform.rotation;
				kinesin.vesicle.gameObject.SetActive( true );
			}

			kinesin.atpGenerator.DestroyAll();
			Destroy( kinesin.gameObject );
		}

		float GetKinesinNormalRotation (Kinesin kinesin)
		{
			Vector3 mtToCargo = kinesin.cargo.transform.position - microtubule.spline.GetPosition( kinesinRange.x );
			Vector3 mtNormal = microtubule.spline.GetNormal( kinesinRange.x );
			return Mathf.Rad2Deg * Mathf.Acos( Mathf.Clamp( Vector3.Dot( mtNormal, mtToCargo.normalized ), -1f, 1f ) );
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