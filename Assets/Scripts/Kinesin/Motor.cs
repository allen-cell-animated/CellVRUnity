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

	[RequireComponent( typeof(Rigidbody), typeof(RandomForces) )]
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

		MeshRenderer _meshRenderer;
		MeshRenderer meshRenderer
		{
			get {
				if (_meshRenderer == null)
				{
					_meshRenderer = GetComponent<MeshRenderer>();
				}
				return _meshRenderer;
			}
		}

		Color color;

		void Start ()
		{
			color = meshRenderer.material.color;
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
			if (state == MotorState.Free)
			{
				meshRenderer.material.color = color;
			}
			else if (state == MotorState.Weak)
			{
				meshRenderer.material.color = Color.yellow;
			}
			else
			{
				meshRenderer.material.color = Color.red;
			}

			if (!pause)
			{
				CheckRelease();
				UpdateNucleotide();
			}
		}

		// ---------------------------------------------- Binding

		public void BindToMT (Tubulin _tubulin)
		{
			if (!neckLinker.bindIsPhysicallyImpossible && !pause)
			{
				tubulin = _tubulin;
				tubulin.hasMotorBound = true;
				state = MotorState.Weak;
				body.isKinematic = true;
				randomForces.addForces = false;
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

		public void Release ()
		{
			tubulin.hasMotorBound = false;
			mover.moving = rotator.rotating = false;
			neckLinker.StopSnapping();
			state = MotorState.Free;
			body.isKinematic = false;
			randomForces.addForces = true;
			binding = false;
		}

		// ---------------------------------------------- State Management

		void CheckRelease ()
		{
			if (bound)
			{
				if (neckLinker.bindIsPhysicallyImpossible && state != MotorState.Strong)
				{
					Debug.Log(name + " released b/c physically impossible");
					Release();
				}
				else if (!binding)
				{
					if (shouldRelease)
					{
						Debug.Log(name + " released w/ probability");
						Release();
					}
				}
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
			return 0;
		}

		// ---------------------------------------------- Nucleotide

		float lastUpdateNucleotideTime = -1f;
		public bool pause;

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
					HydrolyzeATP();
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
				if (state != MotorState.Weak)
				{
					return false;
				}
				else 
				{
					return true;
				}
//				float probability = 0.5f;
//				if (!neckLinker.tensionIsForward)
//				{
//					probability -= neckLinker.tension / kinesin.maxTension;
//				}
//				float random = Random.Range(0, 1f);
//				return random < probability;
			}
		}

		public void BindATP ()
		{
			state = MotorState.Strong;
			atpBindingTime = Time.time;
			neckLinker.StartSnapping();
		}

		void HydrolyzeATP ()
		{
			nucleotide.Hydrolyze();
			state = MotorState.Weak;
			neckLinker.StopSnapping();
		}

		float atpBindingTime = -1f;

		bool shouldHydrolyzeATP
		{
			get {
				if (nucleotide.bound && nucleotide.isATP)
				{
					float probability = (Time.time - atpBindingTime) / kinesin.ATPHydrolysisTime - 0.5f;
					float random = Random.Range(0, 1f);
					return random < probability;
				}
				return false;
			}
		}
	}
}