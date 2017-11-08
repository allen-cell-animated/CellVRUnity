using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public class ResolutionManager : MonoBehaviour 
	{
		public GameObject[] lods;
		public float[] LODDistances;
		public int currentLOD = 0;
		public float distance;

		void Start ()
		{
			SetLOD();
		}

		void SetLOD ()
		{
			distance = Vector3.Distance( transform.position, Camera.main.transform.position );

			if (distance > LODDistances[currentLOD])
			{
				lods[currentLOD].SetActive( false );
			}

			for (int i = 0; i < LODDistances.Length; i++)
			{
				if (distance <= LODDistances[i])
				{
					lods[i].SetActive( true );
					currentLOD = i;
					return;
				}
			}
		}
	}
}