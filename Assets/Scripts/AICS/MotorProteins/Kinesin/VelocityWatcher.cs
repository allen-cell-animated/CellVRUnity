using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins.Kinesin
{
	public class VelocityWatcher : MonoBehaviour 
	{
		Vector3 currentPosition;
		Vector3 lastPosition;

		public Vector3 displacement
		{
			get
			{
				return transform.position - lastPosition;
			}
		}

		void Start ()
		{
			currentPosition = lastPosition = transform.position;
		}

		void Update () 
		{
			lastPosition = currentPosition;
			currentPosition = transform.position;
		}
	}
}