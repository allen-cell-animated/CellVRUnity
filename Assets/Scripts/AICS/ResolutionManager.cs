using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public class ResolutionManager : MonoBehaviour 
	{
		public GameObject[] lods;
		public int currentLOD = 0;

		public void SetLOD (int newLOD)
		{
			if (currentLOD >= 0)
			{
				lods[currentLOD].SetActive( false );
			}
			if (newLOD >= 0)
			{
				lods[newLOD].SetActive( true );
			}
			currentLOD = newLOD;
		}
	}
}