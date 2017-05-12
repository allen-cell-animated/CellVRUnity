using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Tubulin : MonoBehaviour 
	{
		public int type;

		void Start ()
		{
			if (type == 0)
			{
				GetComponent<Collider>().enabled = false;
			}
		}

		void OnCollisionEnter (Collision collision)
		{
			Motor motor = collision.collider.GetComponent<Motor>();
			if (motor != null && !motor.bound)
			{
				motor.BindToMT( this );
			}
		}
	}
}