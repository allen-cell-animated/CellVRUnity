using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Microtubule
{
	public class Tubulin : MonoBehaviour
	{
		public int type = -1;
		public bool hasMotorBound;

//		void Start ()
//		{
//			if (type == 0)
//			{
//				Collider[] colliders = GetComponentsInChildren<Collider>();
//				foreach (Collider collider in colliders)
//				{
//					collider.enabled = false;
//				}
//			}
//		}

		public void Place (Vector3 position, Vector3 lookDirection, Vector3 normal)
		{
			transform.localPosition = position;
			transform.LookAt( transform.position + lookDirection, normal );
		}
	}
}