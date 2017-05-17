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
		public Nucleotide nucleotidePrefab;

		Vector3 bindingPosition = new Vector3( -0.38f, 4.16f, -0.6f );
		Vector3 bindingRotation = new Vector3( -3f, -177f, 0.86f );
		Tubulin tubulin;
		Nucleotide nucleotide;

		bool binding;
		bool bound
		{
			get {
				return state != MotorState.Free;
			}
		}

		Kinesin _kinesin;
		public Kinesin kinesin
		{
			get {
				if (_kinesin == null)
				{
					_kinesin = GetComponentInParent<Kinesin>();
				}
				return _kinesin;
			}
		}

		Necklinker _neckLinker;
		Necklinker neckLinker
		{
			get {
				if (_neckLinker == null)
				{
					_neckLinker = GetComponentInChildren<Necklinker>();
				}
				return _neckLinker;
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

		void Start ()
		{
			CreateNucleotide();
		}

		void CreateNucleotide ()
		{
			if (nucleotidePrefab == null)
			{
				Debug.LogWarning("Nucleotide prefab is missing!");
				return;
			}

			nucleotide = Instantiate<Nucleotide>( nucleotidePrefab, kinesin.transform );
			nucleotide.Init( this );
		}

		void Update ()
		{
			if (neckLinker.stretched) { Debug.Log(name + " stretched!! "); }
			CheckRelease();
		}

		// ---------------------------------------------- Binding

		public void BindToMT (Tubulin _tubulin)
		{
			if (neckLinker.tension < kinesin.tensionToRemoveBoundMotor)
			{
				Debug.Log( name + " bind MT" );
				tubulin = _tubulin;
				tubulin.hasMotorBound = true;
				state = MotorState.Weak;
				body.isKinematic = true;
				randomForces.enabled = false;
				mover.MoveToWithSpeed( tubulin.transform.TransformPoint( bindingPosition ), 15f, FinishBinding );
				rotator.RotateToWithSpeed( GetBindingRotation(), 5f );
				binding = true;
			}
		}

		Quaternion GetBindingRotation ()
		{
			return tubulin.transform.rotation * Quaternion.Euler( bindingRotation );
		}

		void FinishBinding ()
		{
			binding = false;
			Debug.Log("binding finished!");
		}

		void CheckRelease ()
		{
			if (bound)
			{
				if (neckLinker.tension > kinesin.maxTension)
				{
					Release();
				}
				else if (!binding)
				{
					float random = Random.Range(0, 1f);
					float probability = (state == MotorState.Weak) ? ProbabilityOfEjectionFromWeak() : ProbabilityOfEjectionFromStrong();
					if (random < probability)
					{
						Release();
					}
				}
			}
		}

		float ProbabilityOfEjectionFromWeak ()
		{
			return 0.9f / (1f + Mathf.Exp( -10f * (neckLinker.tension - kinesin.tensionToRemoveBoundMotor) ));
		}

		float ProbabilityOfEjectionFromStrong ()
		{
			return 0.1f;
		}

		void Release ()
		{
			Debug.Log( name + " unbind" );
			tubulin.hasMotorBound = false;
			mover.moving = rotator.rotating = false;
			state = MotorState.Free;
			body.isKinematic = false;
			randomForces.enabled = true;
			binding = false;
		}
	}
}