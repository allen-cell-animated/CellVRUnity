using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public class LimitMotion : MonoBehaviour 
	{
		public float maxDisplacementPerSecond = 1f;

		Vector3 lastPosition;

		void Start ()
		{
			lastPosition = transform.position;
		}

		void Update () 
		{
			Vector3 displacement = transform.position - lastPosition;
			transform.position = lastPosition + maxDisplacementPerSecond * Time.deltaTime * Vector3.Normalize( displacement );
			lastPosition = transform.position;
		}
	}
}