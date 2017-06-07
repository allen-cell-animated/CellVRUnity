using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Organelle : MonoBehaviour 
	{
		public Microtubule microtubule;
		public float distanceMoved;

		float startT = -1f;

		void Update () 
		{
			if (microtubule != null && startT < 0)
			{
				startT = microtubule.spline.GetTForClosestPoint( transform.position );
			}
			CalculateDistanceMovedAlongMT();
		}

		void CalculateDistanceMovedAlongMT ()
		{
			if (microtubule != null)
			{
				float currentT = microtubule.spline.GetTForClosestPoint( transform.position );
				distanceMoved = microtubule.spline.GetLength( startT, currentT, 10 );
			}
		}
	}
}