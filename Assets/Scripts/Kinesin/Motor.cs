using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Motor : MonoBehaviour 
	{
		Vector3 bindingPosition = new Vector3( -0.38f, 4.16f, -0.6f );
		Vector3 bindingRotation = new Vector3( -3f, -177f, 0.86f );

		public bool bound;

		RandomForces _randomForces;
		RandomForces randomForces
		{
			get {
				if (_randomForces == null)
				{
					_randomForces = GetComponent<RandomForces>();
				}
				return _randomForces;
			}
		}

		Mover _mover;
		Mover mover
		{
			get {
				if (_mover == null)
				{
					_mover = GetComponent<Mover>();
					if (_mover == null)
					{
						_mover = gameObject.AddComponent<Mover>();
					}
				}
				return _mover;
			}
		}

		Rotator _rotator;
		Rotator rotator
		{
			get {
				if (_rotator == null)
				{
					_rotator = GetComponent<Rotator>();
					if (_rotator == null)
					{
						_rotator = gameObject.AddComponent<Rotator>();
					}
				}
				return _rotator;
			}
		}

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

		public void BindToMT (Tubulin tubulin)
		{
			Debug.Log( name + " bind MT" );
			bound = true;
			body.isKinematic = true;
			randomForces.enabled = false;
			mover.MoveToOverDuration( GetBindingPosition( tubulin.transform ), 0.1f );
			rotator.RotateToOverDuration( GetBindingRotation( tubulin.transform ), 0.1f );
		}

		Vector3 GetBindingPosition (Transform tubulin)
		{
			return tubulin.position + bindingPosition.x * tubulin.right + bindingPosition.y * tubulin.up + bindingPosition.z * tubulin.forward;
		}

		Quaternion GetBindingRotation (Transform tubulin)
		{
			return tubulin.rotation * Quaternion.Euler( bindingRotation );
		}
	}
}