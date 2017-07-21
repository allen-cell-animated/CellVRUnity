using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Necklinker
{
	public class Motor : MonoBehaviour
	{
		public float necklinkerSnappingForce = 5f;

		void Start ()
		{
			SetupNecklinkers();
		}

		void SetupNecklinkers ()
		{
			Necklinker[] neckLinkers = GetComponentsInChildren<Necklinker>();
			Vector3[] dockedLinkPositions = new Vector3[neckLinkers[0].links.Length];
			foreach (Necklinker nL in neckLinkers)
			{
				if (nL.startDocked)
				{
					for (int i = 0; i < nL.links.Length; i++)
					{
						dockedLinkPositions[i] = transform.InverseTransformPoint( nL.links[i].transform.position );
					}
				}
			}

			foreach (Necklinker nL in neckLinkers)
			{
				if (!nL.startDocked)
				{
					nL.SetDockedPositions( dockedLinkPositions );
				}
				else
				{
					nL.gameObject.SetActive( false );
				}
			}
		}
	}
}