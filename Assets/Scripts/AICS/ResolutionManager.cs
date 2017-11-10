using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public class ResolutionManager : MonoBehaviour 
	{
		public bool updateAfterStart = false;
		public float updateInterval;
		public GameObject[] lods;
		public float[] LODDistances;
		public int currentLOD = 0;

		float distance;
		float lastTime;

		void Start ()
		{
			SetLOD();
			lastTime = Time.time;
		}

		void Update ()
		{
			if (updateAfterStart && Time.time - lastTime >= updateInterval)
			{
				SetLOD();
				lastTime = Time.time;
			}
		}

		void SetLOD ()
		{
			distance = Vector3.Distance( transform.position, Camera.main.transform.position );

			if ((currentLOD > 0 && distance <= LODDistances[currentLOD - 1]) || (currentLOD < LODDistances.Length && distance > LODDistances[currentLOD]))
			{
				ShowLOD( currentLOD, false );

				for (int i = 0; i < LODDistances.Length; i++)
				{
					if (distance <= LODDistances[i])
					{
						ShowLOD( i, true );
						currentLOD = i;
						return;
					}
				}
				currentLOD = LODDistances.Length;
			}
		}

		void ShowLOD (int index, bool show)
		{
			if (index >= 0 && index < lods.Length) 
			{
				lods[index].SetActive( show );
			}
		}
	}
}