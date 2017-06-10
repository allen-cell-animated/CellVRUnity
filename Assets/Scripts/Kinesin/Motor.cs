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
		public bool checkBindingOrientation = true;
		public MotorState state = MotorState.Free;
		public bool pause; // for testing
		public bool binding; // public for testing
		public bool releasing; // public for testing
		public bool shouldReleaseNecklinker;
		public bool inFront; // for testing

		float bindTime = 0.7f;
		float bindStartTime = -1f;
		float bindingForce = 100f;
		float lastCheckReleaseTime = -1f;
		Vector3 bindingPosition = new Vector3( -0.38f, 4.16f, -0.6f );
		Vector3 bindingRotation = new Vector3( -3f, -177f, 0.86f );
		Vector3 bindingRotationTolerance = new Vector3( 30f, 30f, 20f );
		Tubulin tubulin;
		Color color;

		public bool bound
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
		MeshRenderer meshRenderer // testing
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
				meshRenderer.material.color = new Color( 1f, 0.5f, 0 );
			}
			else
			{
				meshRenderer.material.color = Color.red;
			}

			inFront = !neckLinker.tensionIsForward;

			if (!pause)
			{
				UpdateBinding();
				UpdateRelease();
				UpdateNucleotideProbabilities();
			}
		}

		// ---------------------------------------------- Binding

		void OnCollisionEnter (Collision collision)
		{
			if (state == MotorState.Free)
			{
				Tubulin _tubulin = collision.collider.GetComponentInParent<Tubulin>();
				if (_tubulin != null && !_tubulin.hasMotorBound)
				{
					BindToMT( _tubulin );
				}
			}
		}

		void BindToMT (Tubulin _tubulin)
		{
			if (!neckLinker.bindIsPhysicallyImpossible && closeToBindingOrientation( _tubulin ) && !pause)
			{
				Debug.Log(name + " bind");
				tubulin = _tubulin;
				tubulin.hasMotorBound = true;
				state = MotorState.Weak;
				randomForces.addForce = randomForces.addTorque = false;
				attractor.attractiveForce = 0;
				attractor.target = tubulin.transform;
				rotator.RotateToOverDuration( GetBindingRotation(), bindTime );
				bindStartTime = Time.time;
				binding = true;
			}
		}

		bool closeToBindingOrientation (Tubulin _tubulin)
		{
			if (!checkBindingOrientation || kinesin.OtherMotor( this ).bound)
			{
				return true;
			}
			else 
			{
				Vector3 localRotation = (Quaternion.Inverse( _tubulin.transform.rotation ) * transform.rotation).eulerAngles;
				return Helpers.AngleIsWithinTolerance( localRotation.x, bindingRotation.x, bindingRotationTolerance.x )
					&& Helpers.AngleIsWithinTolerance( localRotation.y, bindingRotation.y, bindingRotationTolerance.y )
					&& Helpers.AngleIsWithinTolerance( localRotation.z, bindingRotation.z, bindingRotationTolerance.z );
			}
		}

		void UpdateBinding ()
		{
			if (binding || releasing)
			{
				EaseBind();
			}
		}

		void EaseBind ()
		{
			if (Time.time - bindStartTime < bindTime)
			{
				if (binding)
				{
					attractor.attractiveForce = bindingForce * (Time.time - bindStartTime) / bindTime;
				}
				else if (releasing)
				{
					attractor.attractiveForce = bindingForce * (1f - (Time.time - bindStartTime) / bindTime);
				}
			}
			else
			{
				attractor.target = null;
				if (binding)
				{
					rotator.SnapToGoal();
					body.isKinematic = true;
					transform.position = tubulin.transform.TransformPoint( bindingPosition );
					if (atpBinder.nucleotide != null && atpBinder.nucleotide.isATP)
					{
						BindATP();
					}
					binding = false;
				}
				else if (releasing)
				{
					tubulin.hasMotorBound = false;
					state = MotorState.Free;
					randomForces.addForce = randomForces.addTorque = true;
					releasing = false;
				}
			}
		}

		Quaternion GetBindingRotation ()
		{
			return tubulin.transform.rotation * Quaternion.Euler( bindingRotation );
		}

		// ---------------------------------------------- Releasing

		void UpdateRelease ()
		{
			if (state != MotorState.Strong && shouldReleaseNecklinker)
			{
				neckLinker.Release();
				shouldReleaseNecklinker = false;
			}

			if (bound && !binding && !releasing)
			{
				if (shouldRelease)
				{
					Debug.Log(name + " released w/ probability in state " + state.ToString());
					Release();
				}
			}
		}

		bool shouldRelease
		{
			get {
				if (Time.time - lastCheckReleaseTime > 0.3f)
				{
					lastCheckReleaseTime = Time.time;
					float probability = probabilityOfEjectionFromStrong;
					if (state == MotorState.Weak) 
					{
						probability = (neckLinker.tensionIsForward) ? probabilityOfEjectionFromWeak : 0.05f; // this is the back motor
					}
					float random = Random.Range(0, 1f);
					return random <= probability;
				}
				return false;
			}
		}

		float probabilityOfEjectionFromWeak
		{
			get {
				return 0.001f * 0.9f / (1f + Mathf.Exp( -10f * (neckLinker.tension - kinesin.tensionToRemoveWeaklyBoundMotor) ));
			}
		}

		float probabilityOfEjectionFromStrong
		{
			get {
				return 0;
			}
		}

		public void ReleaseFromTension (string releaserName)
		{
			if (state == MotorState.Strong)
			{
				Debug.Log(neckLinker.tension);
			}
			if (state == MotorState.Weak || (state == MotorState.Strong && neckLinker.tension > 1f))
			{
				Debug.Log( releaserName + " released " + name );
				Release();
			}
		}

		void Release ()
		{
			rotator.rotating = false;
			neckLinker.Release();
			binding = false;
			releasing = true;
			body.isKinematic = false;
			attractor.attractiveForce = bindingForce;
			attractor.target = tubulin.transform;
			bindStartTime = Time.time;
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
				kinesin.OtherMotor( this ).shouldReleaseNecklinker = true;
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
			float probability = 0.001f;
			if (!neckLinker.tensionIsForward) // this is the front motor
			{
				probability = 1f - 2.5f * (neckLinker.tension / kinesin.maxTension - 0.4f); // p = 0 at high tension (0.8), p = 1 at low tension (0.4)
			}
			atpBinder.ATPBindingProbability = probability;
		}

		void UpdateADPReleaseProbability ()
		{
			float probability = 0.1f;
			if (neckLinker.tensionIsForward) // this is the back motor
			{
				probability = 1f - 2.5f * (neckLinker.tension / kinesin.maxTension - 0.4f);  // p = 0 at high tension (0.8), p = 1 at low tension (0.4)
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