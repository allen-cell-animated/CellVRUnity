using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins
{
	public class ResolutionNode : MonoBehaviour 
	{
		MolecularEnvironment environment;
		List<ResolutionManager> objectsInNode = new List<ResolutionManager>();

		int currentLOD = 0;
		float updateInterval = 2f;
		float lastTime = -1f;
		float lastDistance = 20f;

		public void Setup (MolecularEnvironment _environment)
		{
			environment = _environment;
			Destroy( GetComponent<MeshRenderer>() );
			BoxCollider collider = GetComponent<BoxCollider>();
			collider.isTrigger = true;
			collider.size = environment.gridSize * Vector3.one;
			Rigidbody body = gameObject.AddComponent<Rigidbody>();
			body.useGravity = false;
			body.isKinematic = true;
		}

		void OnTriggerEnter (Collider other)
		{
			ResolutionManager obj = other.GetComponentInParent<ResolutionManager>();
			if (obj != null && !objectsInNode.Contains( obj ))
			{
				objectsInNode.Add( obj );
				obj.SetLOD( currentLOD );
			}
		}

		void OnTriggerExit (Collider other)
		{
			ResolutionManager obj = other.GetComponentInParent<ResolutionManager>();
			if (obj != null && objectsInNode.Contains( obj ))
			{
				objectsInNode.Remove( obj );
			}
		}

		void Update () 
		{
			if (Time.time - lastTime > updateInterval)
			{
				CheckLOD();
				lastTime = Time.time;
			}
		}

		void CheckLOD ()
		{
			lastDistance = Vector3.Distance( transform.position, Camera.main.transform.position );

			if (lastDistance > environment.LODDistances[environment.LODDistances.Length - 1])
			{
				if (currentLOD != -1)
				{
					SwitchLOD( -1 );
				}
				return;
			}

			for (int i = 0; i < environment.LODDistances.Length; i++)
			{
				if (environment.LODDistances[i] <= 0 || lastDistance < environment.LODDistances[i])
				{
					if (currentLOD != i)
					{
						SwitchLOD( i );
					}
					return;
				}
			}
		}

		void SwitchLOD (int newLOD)
		{
			foreach (ResolutionManager obj in objectsInNode)
			{
				obj.SetLOD( newLOD );
			}
			currentLOD = newLOD;
		}
	}
}