using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	[RequireComponent( typeof(Rigidbody) )]
	public class Friction : MonoBehaviour 
	{
		public float magnitude = 0.5f;

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
			body.AddForce( magnitude * -body.velocity );
		}
	}
}