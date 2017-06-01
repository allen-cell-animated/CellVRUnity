using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Organelle : MonoBehaviour 
	{
		public float distanceMoved;

		Vector3 startPosition;

		void Start () 
		{
			startPosition = transform.position;
		}

		void Update () 
		{
			distanceMoved = Vector3.Distance( startPosition, transform.position );
		}
	}
}