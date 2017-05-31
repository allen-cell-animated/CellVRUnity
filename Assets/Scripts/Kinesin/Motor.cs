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

	[RequireComponent( typeof(Rigidbody), typeof(RandomForces), typeof(ATPBinder) )]
	public class Motor : MonoBehaviour, IBindATP
	{
		public MotorState state = MotorState.Free;
		public bool pause;

		Vector3 bindingPosition = new Vector3( -0.38f, 4.16f, -0.6f );
		Vector3 bindingRotation = new Vector3( -3f, -177f, 0.86f );
		Tubulin tubulin;
		Color color;

		bool binding;
		bool bound
		{
			get {
				return state != MotorState.Free && !releasing;
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

		Attractor _attractor;
		Attractor attractor
		{
			get {
				if (_attractor == null)
				{
					_attractor = GetComponent<Attractor>();
					if (_attractor == null)
					{
						_attractor = gameObject.AddComponent<Attractor>();
					}
				}
				return _attractor;
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

		void Start ()
		{
			color = meshRenderer.material.color;
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
				UpdateNucleotideProbabilities();
			}
		}

		// ---------------------------------------------- Binding

		public void BindToMT (Tubulin _tubulin)
		{
			if (!neckLinker.bindIsPhysicallyImpossible && !bound && !pause)
			{
				Debug.Log(name + " bind");
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
			if (bound)
			{
				Debug.Log(name + " start release");
				mover.moving = rotator.rotating = false;
				neckLinker.StopSnapping();
				binding = false;
				releasing = true;
				body.isKinematic = false;
				attractor.target = tubulin.transform;
				attractor.attractiveForce = releasingForce;
				releaseStartTime = Time.time;
			}
		}

		public bool releasing;
		float releaseTime = 0.1f;
		float releaseStartTime = -1f;
		float releasingForce = 1000f;

		void EaseRelease ()
		{
			if (Time.time - releaseStartTime < releaseTime)
			{
				attractor.attractiveForce = releasingForce * (1f - (Time.time - releaseStartTime) / releaseTime);
			}
			else
			{
				Debug.Log(name + " finish release");
				attractor.target = null;
				tubulin.hasMotorBound = false;
				state = MotorState.Free;
				randomForces.addForces = true;
				releasing = false;
			}
		}

		// ---------------------------------------------- State Management

		void CheckRelease ()
		{
			if (releasing)
			{
				EaseRelease();
			}
			else if (bound)
			{
//				if (neckLinker.bindIsPhysicallyImpossible && state != MotorState.Strong)
//				{
//					Debug.Log(name + " released b/c physically impossible");
//					Release();
//				} else
				if (!binding)
				{
					if (shouldRelease)
					{
						Debug.Log(name + " released w/ probability " + state.ToString());
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
					float probability = 0;
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

		ATPBinder _atpBinder;
		ATPBinder atpBinder
		{
			get {
				if (_atpBinder == null)
				{
					_atpBinder = GetComponent<ATPBinder>();
				}
				return _atpBinder;
			}
		}

		public void BindATP ()
		{
			Debug.Log(name + " Bind ATP");
			if (bound)
			{
				state = MotorState.Strong;
				neckLinker.StartSnapping();
			}
		}

		void UpdateNucleotideProbabilities ()
		{
			UpdateATPBindingProbability();
			UpdateADPReleaseProbability();
		}

		void UpdateATPBindingProbability ()
		{
			if (state != MotorState.Weak || releasing)
			{
				atpBinder.ATPBindingProbability = 0;
			}
			else 
			{
				atpBinder.ATPBindingProbability = 1f;
			}
//			float probability = 0.5f;
//			if (!neckLinker.tensionIsForward)
//			{
//				probability -= neckLinker.tension / kinesin.maxTension;
//			}
//			atpBinder.ATPBindingProbability = probability;
		}

		void UpdateADPReleaseProbability ()
		{
			float probability = 0.5f;
			if (neckLinker.tensionIsForward)
			{
				probability -= neckLinker.tension / kinesin.maxTension;
			}
			atpBinder.ADPReleaseProbability = probability;
		}

		public void HydrolyzeATP ()
		{
			Debug.Log(name + " Hydrolyze");
			neckLinker.StopSnapping();
			if (bound)
			{
				state = MotorState.Weak;
			}
		}
	}
}