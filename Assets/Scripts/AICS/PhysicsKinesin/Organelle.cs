using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MT;

namespace AICS.PhysicsKinesin
{
	public class Organelle : MonoBehaviour 
	{
		public float avoidMTForce = 100f;

		Rigidbody _body;
		Rigidbody body
		{
			get {
				if (_body == null)
				{
					_body = GetComponent<Rigidbody>();
				}
				return _body;
			}
		}

		void OnTriggerStay (Collider other)
		{
			Tubulin tubulin = other.GetComponentInParent<Tubulin>();
			if (tubulin != null)
			{
				body.AddForce( avoidMTForce * tubulin.transform.up );
			}
		}
	}
}