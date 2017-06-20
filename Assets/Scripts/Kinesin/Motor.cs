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
		public bool startWithDockedNecklinker;
		public bool pause; // for testing
		public bool binding; // public for testing
		public bool releasing; // public for testing
		public bool shouldReleaseNecklinker;
		public bool inFront; // for testing

		float bindTime = 0.7f;
		float bindStartTime = -1f;
		float bindingForce = 500f;
		float lastCheckReleaseTime = -1f;
		Vector3 bindingPosition = new Vector3( 0.34f, 4.01f, 0.34f );
		Vector3 bindingRotation = new Vector3( -2.27f, -90.52f, 180.221f );
//		Vector3 bindingRotationTolerance = new Vector3( 30f, 30f, 20f );
		public Tubulin tubulin;
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
		public Necklinker neckLinker
		{
			get {
				if (_neckLinker == null)
				{
					SetupNecklinkers();
				}
				return _neckLinker;
			}
		}

		void SetupNecklinkers ()
		{
			Necklinker[] neckLinkers = GetComponentsInChildren<Necklinker>();
			Vector3[] dockedLinkPositions = new Vector3[neckLinkers[0].links.Length];
			Quaternion[] dockedLinkRotations = new Quaternion[neckLinkers[0].links.Length];
			foreach (Necklinker nL in neckLinkers)
			{
				if (nL.startDocked)
				{
					for (int i = 0; i < nL.links.Length; i++)
					{
						dockedLinkPositions[i] = transform.InverseTransformPoint( nL.links[i].transform.position );
						dockedLinkRotations[i] = nL.links[i].transform.localRotation;
					}
				}
			}

			foreach (Necklinker nL in neckLinkers)
			{
				if ((nL.startDocked && startWithDockedNecklinker) || (!nL.startDocked && !startWithDockedNecklinker))
				{
					_neckLinker = nL;
					_neckLinker.SetDockedTransforms( dockedLinkPositions, dockedLinkRotations );
				}
				else
				{
					nL.gameObject.SetActive( false );
				}
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
					_meshRenderer = GetComponent<ResolutionManager>().lods[0].geometry.GetComponent<MeshRenderer>();
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
				body.constraints = RigidbodyConstraints.FreezeRotation;
				rotator.RotateToOverDuration( GetBindingRotation(), bindTime );
				bindStartTime = Time.time;
				binding = true;
			}
		}

		bool closeToBindingOrientation (Tubulin _tubulin)
		{
			return true;
//			if (!checkBindingOrientation || kinesin.OtherMotor( this ).bound)
//			{
//				return true;
//			}
//			else 
//			{
//				Vector3 localRotation = (Quaternion.Inverse( _tubulin.transform.rotation ) * transform.rotation).eulerAngles;
//				return Helpers.AngleIsWithinTolerance( localRotation.x, bindingRotation.x, bindingRotationTolerance.x )
//					&& Helpers.AngleIsWithinTolerance( localRotation.y, bindingRotation.y, bindingRotationTolerance.y )
//					&& Helpers.AngleIsWithinTolerance( localRotation.z, bindingRotation.z, bindingRotationTolerance.z );
//			}
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
				if (releasing)
				{
					Debug.Log( "finish release" );
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
			if (!releasing && (state == MotorState.Weak || (state == MotorState.Strong && neckLinker.tension > 1f)))
			{
				Debug.Log( releaserName + " released " + name + " in state " + state);
				Release();
			}
		}

		void Release ()
		{
			rotator.rotating = false;
			neckLinker.Release();
			binding = false;
			releasing = true;
			body.constraints = RigidbodyConstraints.None;
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
			atpBinder.ATPBindingProbability = 1;// probability;
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