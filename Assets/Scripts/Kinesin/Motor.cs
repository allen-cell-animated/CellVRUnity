using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public enum MotorState
	{
		Free,
		Weak,
		Strong
	}

	public class Motor : MonoBehaviour 
	{
		public MotorState state = MotorState.Free;
		
		float[] linkerLengthExtents = new float[]{1f, 8f};
		Vector3 bindingPosition = new Vector3( -0.38f, 4.16f, -0.6f );
		Vector3 bindingRotation = new Vector3( -3f, -177f, 0.86f );
		Tubulin tubulin;

		bool bound
		{
			get {
				return state != MotorState.Free;
			}
		}

		Kinesin _kinesin;
		Kinesin kinesin
		{
			get {
				if (_kinesin == null)
				{
					_kinesin = GetComponentInParent<Kinesin>();
				}
				return _kinesin;
			}
		}

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

		void Update ()
		{
			CheckUnbind();
		}

		// ---------------------------------------------- Binding

		public void BindToMT (Tubulin _tubulin)
		{
			if (neckLinkerTension < kinesin.tensionToRemoveBoundMotor)
			{
				Debug.Log( name + " bind MT" );
				tubulin = _tubulin;
				tubulin.hasMotorBound = true;
				state = MotorState.Weak;
				body.isKinematic = true;
				randomForces.enabled = false;
				mover.MoveToOverDuration( GetBindingPosition(), 0.1f );
				rotator.RotateToOverDuration( GetBindingRotation(), 0.1f );
			}
		}

		Vector3 GetBindingPosition ()
		{
			return tubulin.transform.position + bindingPosition.x * tubulin.transform.right 
				+ bindingPosition.y * tubulin.transform.up + bindingPosition.z * tubulin.transform.forward;
		}

		Quaternion GetBindingRotation ()
		{
			return tubulin.transform.rotation * Quaternion.Euler( bindingRotation );
		}

		void CheckUnbind ()
		{
			if (bound)
			{
				float random = Random.Range(0, 1f);
				float probability = (state == MotorState.Weak) ? ProbabilityOfEjectionFromWeak() : ProbabilityOfEjectionFromStrong();
				if (random < probability || neckLinkerTension > 0.8f)
				{
					Unbind();
				}
			}
		}

		float ProbabilityOfEjectionFromWeak ()
		{
			return 0.9f / (1f + Mathf.Exp( -10f * (neckLinkerTension - kinesin.tensionToRemoveBoundMotor) ));
		}

		float ProbabilityOfEjectionFromStrong ()
		{
			return 0.1f;
		}

		float neckLinkerTension
		{
			get {
				float linkerLength = Vector3.Distance( transform.position, kinesin.hips.transform.position );
				return (linkerLength - linkerLengthExtents[0]) / (linkerLengthExtents[1] - linkerLengthExtents[0]);
			}
		}

		void Unbind ()
		{
			Debug.Log( name + " unbind" );
			tubulin.hasMotorBound = false;
			mover.moving = rotator.rotating = false;
			state = MotorState.Free;
			body.isKinematic = false;
			randomForces.enabled = true;
		}
	}
}