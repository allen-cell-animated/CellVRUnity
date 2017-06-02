using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Organelle : MonoBehaviour 
	{
		public Microtubule microtubule;
		public float distanceMoved;

		float startT;

		void Start () 
		{
			if (microtubule != null)
			{
				startT = microtubule.spline.GetTForClosestPoint( transform.position );
			}
		}

		void Update () 
		{
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