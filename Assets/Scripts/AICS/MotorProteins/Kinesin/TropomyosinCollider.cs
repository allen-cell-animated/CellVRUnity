using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins.Kinesin
{
	public class TropomyosinCollider : MonoBehaviour 
	{
		Rigidbody _body;
		protected Rigidbody body
		{
			get
			{
				if (_body == null)
				{
					_body = GetComponent<Rigidbody>();
					_body.useGravity = false;
					_body.isKinematic = true;
				}
				return _body;
			}
		}

		void OnTriggerEnter (Collider other)
		{
			if (other.tag == "Player")
			{
				RaycastHit hit;
				if (Physics.Raycast( other.transform.position, transform.position - other.transform.position, out hit ))
				{
					body.AddForce( hit.point - other.transform.position, ForceMode.Impulse );
				}
			}
		}
	}
}