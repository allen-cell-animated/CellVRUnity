using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Tubulin : MonoBehaviour, ICompoundStaticColliderParent
	{
		public int type;
		public bool hasMotorBound;

		void Start ()
		{
			if (type == 0)
			{
				Collider[] colliders = GetComponentsInChildren<Collider>();
				foreach (Collider collider in colliders)
				{
					collider.enabled = false;
				}
			}
		}

		void OnCollisionEnter (Collision collision)
		{
			Collided( collision );
		}

		public void Collided (Collision collision)
		{
			Motor motor = collision.collider.GetComponentInParent<Motor>();
			if (!hasMotorBound && motor != null && motor.state == MotorState.Free)
			{
				motor.BindToMT( this );
			}
		}

		public void Place (Vector3 position, Vector3 lookDirection, Vector3 normal)
		{
			transform.localPosition = position;
			transform.LookAt( transform.position + lookDirection, normal );
		}
	}
}