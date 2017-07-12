using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	[RequireComponent( typeof(Rigidbody) )]
	public class Friction : MonoBehaviour 
	{
		public bool addForce = true;
		public bool addTorque = true;
		public float multiplier = 0.1f;

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

		void Update () 
		{
			if (addForce) { body.AddForce( Mathf.Min( 1E8f, Mathf.Exp( multiplier * body.velocity.magnitude ) ) * Vector3.Normalize( -body.velocity ) ); }
			if (addTorque) { body.AddTorque( Mathf.Min( 1E8f, Mathf.Exp( multiplier * body.angularVelocity.magnitude ) ) * Vector3.Normalize( -body.angularVelocity ) ); }
		}
	}
}