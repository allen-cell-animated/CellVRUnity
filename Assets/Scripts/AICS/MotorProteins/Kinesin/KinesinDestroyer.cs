using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.PhysicsKinesin;

namespace AICS.MotorProteins.Kinesin
{
	public class KinesinDestroyer : MonoBehaviour 
	{
		public KinesinSpawner spawner;

		void OnTriggerEnter (Collider other)
		{
			Hips hips = other.GetComponent<Hips>();
			if (hips != null)
			{
				spawner.KinesinWasDestroyed( hips.kinesin );
				hips.GetComponent<NucleotideGenerator>().DestroyAll();
				Destroy( hips.kinesin.gameObject );
			}
		}
	}
}