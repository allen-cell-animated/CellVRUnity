using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class MotorTest : MonoBehaviour
	{
		public float necklinkerSnappingForce = 5f;

		void Start ()
		{
			SetupNecklinkers();
		}

		void SetupNecklinkers ()
		{
			NecklinkerTest[] neckLinkers = GetComponentsInChildren<NecklinkerTest>();
			Vector3[] dockedLinkPositions = new Vector3[neckLinkers[0].links.Length];
			foreach (NecklinkerTest nL in neckLinkers)
			{
				if (nL.startDocked)
				{
					for (int i = 0; i < nL.links.Length; i++)
					{
						dockedLinkPositions[i] = transform.InverseTransformPoint( nL.links[i].transform.position );
					}
				}
			}

			foreach (NecklinkerTest nL in neckLinkers)
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