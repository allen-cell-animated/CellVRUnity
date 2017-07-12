using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class PhysicsOrganelle : MonoBehaviour 
	{
		public float maxDisplacementPerSecond = 1f;

		Vector3 lastPosition;

		void Start ()
		{
			lastPosition = transform.position;
		}

		void Update () 
		{
			LimitMotion();
		}

		void LimitMotion ()
		{
			Vector3 displacement = transform.position - lastPosition;
			transform.position = lastPosition + maxDisplacementPerSecond * Time.deltaTime * Vector3.Normalize( displacement );
			lastPosition = transform.position;
		}
	}
}