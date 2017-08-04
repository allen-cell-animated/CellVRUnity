using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.AnimatedKinesin
{
	public class TubulinDetector : MonoBehaviour 
	{
		public float tubulinRadius = 3f;
		public List<Tubulin> nearbyTubulins = new List<Tubulin>();

		void OnTriggerEnter (Collider other)
		{
			Tubulin tubulin = other.GetComponent<Tubulin>();
			if (tubulin != null && !nearbyTubulins.Contains( tubulin ))
			{
				nearbyTubulins.Add( tubulin );
			}
		}

		void OnTriggerExit (Collider other)
		{
			Tubulin tubulin = other.GetComponent<Tubulin>();
			if (tubulin != null && nearbyTubulins.Contains( tubulin ))
			{
				nearbyTubulins.Remove( tubulin );
			}
		}

		public Tubulin[] GetCollidingTubulins (Vector3 position, float radius)
		{
			List<Tubulin> collidingTubulins = new List<Tubulin>();
			foreach (Tubulin tubulin in nearbyTubulins)
			{
				if (Vector3.Distance( tubulin.transform.position, position ) <= tubulinRadius + radius - 0.2f)
				{
					collidingTubulins.Add( tubulin );
				}
			}
			return collidingTubulins.ToArray();
		}
	}
}