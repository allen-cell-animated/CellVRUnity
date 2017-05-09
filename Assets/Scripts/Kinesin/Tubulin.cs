using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Tubulin : MonoBehaviour 
	{
		public int type;

		void OnCollisionEnter (Collision collision)
		{
			if (type == 1)
			{
				Attractor motor = collision.collider.GetComponent<Attractor>();
				if (motor != null && motor.target != transform)
				{
					Debug.Log("motor collided with " + name);
					motor.target = transform;
					motor.GetComponent<RandomForces>().enabled = false;
				}
			}
		}
	}
}