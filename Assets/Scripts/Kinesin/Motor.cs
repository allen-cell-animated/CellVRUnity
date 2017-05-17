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

		Color color;

		void Start ()
		{
			color = GetComponent<MeshRenderer>().material.color;
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
			if (neckLinker.tensionIsForward)
			{
				GetComponent<MeshRenderer>().material.color = Color.red;
			}
			else
			{
				GetComponent<MeshRenderer>().material.color = color;
			}
				
			CheckRelease();
			UpdateNucleotide();
		}

		// ---------------------------------------------- Binding

		public void BindToMT (Tubulin _tubulin)
		{
			if (!bindIsPhysicallyImpossible)
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

		// ---------------------------------------------- State Management

		void CheckRelease ()
		{
			if (bound)
			{
				if (bindIsPhysicallyImpossible)
				{
					Release();
				}
				else if (!binding)
				{
					if (shouldRelease)
					{
						Release();
					}
				}
			}
		}

		bool bindIsPhysicallyImpossible
		{
			get {
				return neckLinker.tension > kinesin.maxTension || neckLinker.stretched;
			}
		}

		float lastCheckReleaseTime = -1f;

		bool shouldRelease
		{
			get {
				if (Time.time - lastCheckReleaseTime > 1f) // only check once per second
				{
					lastCheckReleaseTime = Time.time;
					float probability = 0.1f;
					if (neckLinker.tensionIsForward) // this is the back motor
					{
						probability = (state == MotorState.Weak) ? ProbabilityOfEjectionFromWeak() : ProbabilityOfEjectionFromStrong();
					}
					float random = Random.Range(0, 1f);
					return random < probability;
				}
				return false;
			}
		}

		float ProbabilityOfEjectionFromWeak ()
		{
			return 0.9f / (1f + Mathf.Exp( -10f * (neckLinker.tension - kinesin.tensionToRemoveWeaklyBoundMotor) ));
		}

		float ProbabilityOfEjectionFromStrong ()
		{
			return 0.1f;
		}

		// ---------------------------------------------- Nucleotide

		float lastUpdateNucleotideTime = -1f;

		void UpdateNucleotide ()
		{
			if (Time.time - lastUpdateNucleotideTime > 1f) // only check once per second
			{
				if (shouldReleaseADP)
				{
					nucleotide.ReleaseADP();
					nucleotide.Invoke("StartATPBinding", 3f);
				}
				else if (shouldHydrolyzeATP)
				{
					nucleotide.Hydrolyze();
					state = MotorState.Weak;
				}
				lastUpdateNucleotideTime = Time.time;
			}
		}

		bool shouldReleaseADP
		{
			get {
				if (nucleotide.bound && !nucleotide.isATP)
				{
					float probability = 0.5f;
					if (neckLinker.tensionIsForward)
					{
						probability -= neckLinker.tension / kinesin.maxTension;
					}
					float random = Random.Range(0, 1f);
					return random < probability;
				}
				return false;
			}
		}

		public bool shouldATPBind 
		{
			get {
				float probability = 0.5f;
				if (!neckLinker.tensionIsForward)
				{
					probability -= neckLinker.tension / kinesin.maxTension;
				}
				float random = Random.Range(0, 1f);
				return random < probability;
			}
		}

		float atpBindingTime = -1f;

		bool shouldHydrolyzeATP
		{
			get {
				if (nucleotide.bound && nucleotide.isATP)
				{
					float probability = (Time.time - atpBindingTime) / kinesin.atpHydrolysisTime - 0.5f;
					float random = Random.Range(0, 1f);
					return random < probability;
				}
				return false;
			}
		}
	}
}