using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	[RequireComponent( typeof(Rigidbody) )]
	public class Organelle : MonoBehaviour 
	{
		float multiplier = 0.1f;

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
			body.AddForce( Mathf.Min( 1E28f, Mathf.Exp( multiplier * body.velocity.magnitude ) ) * Vector3.Normalize( -body.velocity ) );
		}
	}
}