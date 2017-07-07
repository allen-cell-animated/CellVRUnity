using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

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
		public bool printEvents;
		public MotorState state = MotorState.Free;
		public bool startWithDockedNecklinker;
		public bool shouldReleaseNecklinker;
		public float bindingRotationTolerance = 30f;

		//for testing
		public bool checkBindingOrientation = true; 
		public bool pause; 

		public bool binding; 
		public bool releasing;
		float bindTime = 0.7f;
		float bindStartTime = -1f;
		float bindingForce = 200f;
		Vector3 bindingPosition = new Vector3( 0.34f, 4.01f, 0.34f );
		Vector3 bindingRotation = new Vector3( -2.27f, -90.52f, 180.221f );
		public Tubulin tubulin;
		Color color;

		public bool bound
		{
			get {
				return state != MotorState.Free && !releasing;
			}
		}

		public bool inFront
		{
			get {
				return !neckLinker.tensionIsForward;
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

		Motor _otherMotor;
		public Motor otherMotor
		{
			get
			{
				if (_otherMotor == null)
				{
					_otherMotor = kinesin.OtherMotor( this );
				}
				return _otherMotor;
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
			foreach (Necklinker nL in neckLinkers)
			{
				if (nL.startDocked)
				{
					for (int i = 0; i < nL.links.Length; i++)
					{
						dockedLinkPositions[i] = transform.InverseTransformPoint( nL.links[i].transform.position );
					}
				}
			}

			foreach (Necklinker nL in neckLinkers)
			{
				if ((nL.startDocked && startWithDockedNecklinker) || (!nL.startDocked && !startWithDockedNecklinker))
				{
					_neckLinker = nL;
					_neckLinker.SetDockedPositions( dockedLinkPositions );
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

		Attractor _bindingAttractor;
		Attractor bindingAttractor
		{
			get {
				if (_bindingAttractor == null)
				{
					_bindingAttractor = gameObject.AddComponent<Attractor>();
				}
				return _bindingAttractor;
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

		MeshRenderer[] _colliderRenderers;
		MeshRenderer[] colliderRenderers // testing
		{
			get {
				if (_colliderRenderers == null)
				{
					_colliderRenderers = transform.FindChild( "Collider" ).GetComponentsInChildren<MeshRenderer>();
				}
				return _colliderRenderers;
			}
		}

		void Start ()
		{
			color = meshRenderer.material.color;
		}

		void Update ()
		{
			if (!pause)
			{
				UpdateBindingAnimation();
				UpdateCheckRelease();
				UpdateCheckNecklinker();
				UpdatePushForward();
				UpdateNucleotideProbabilities();
			}

			SetColor();
		}

		void SetColor ()
		{
			if (Time.time - lastATPTime < 0.05f)
			{
				meshRenderer.material.color = new Color( 1f, 0, 1f );
//				SetColliderColor( new Color( 1f, 0, 1f ) );
			}
			else if (state == MotorState.Free)
			{
				meshRenderer.material.color = color;
//				SetColliderColor( color );
			}
			else if (state == MotorState.Weak)
			{
				meshRenderer.material.color = new Color( 1f, 0.5f, 0 );
//				SetColliderColor( new Color( 1f, 0.5f, 0 ) );
			}
			else
			{
				meshRenderer.material.color = Color.red;
//				SetColliderColor( Color.red );
			}
		}

		void SetColliderColor (Color _color)
		{
			foreach (MeshRenderer renderer in colliderRenderers)
			{
				renderer.material.color = _color;
			}
		}

		// ---------------------------------------------- Binding

		void OnCollisionEnter (Collision collision)
		{
			if (state == MotorState.Free && otherMotor.state != MotorState.Strong)
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
			if (!necklinkerWillBeStretched( _tubulin ) && closeToBindingOrientation( _tubulin ) && !pause)
			{
				if (printEvents) { Debug.Log( name + " bind" ); }
				tubulin = _tubulin;
				tubulin.hasMotorBound = true;
				state = MotorState.Weak;
				randomForces.addForce = randomForces.addTorque = false;
				bindingAttractor.GoToTransform( tubulin.transform, 0 );
				body.constraints = RigidbodyConstraints.FreezeRotation;
				rotator.RotateToOverDuration( GetBindingRotation(), bindTime / 2f );
				bindStartTime = Time.time;
				binding = true;
			}
		}

		bool necklinkerWillBeStretched (Tubulin _tubulin)
		{
			return Vector3.Distance( _tubulin.transform.TransformPoint( bindingPosition ), kinesin.hips.transform.position ) > 6f;
		}

		bool closeToBindingOrientation (Tubulin _tubulin)
		{
			if (!checkBindingOrientation) // || otherMotor.bound
			{
				return true;
			}
			else 
			{
				Vector3 localRotation = (Quaternion.Inverse( _tubulin.transform.rotation ) * transform.rotation).eulerAngles;
				return Helpers.AngleIsWithinTolerance( localRotation.x, bindingRotation.x, bindingRotationTolerance )
					&& Helpers.AngleIsWithinTolerance( localRotation.y, bindingRotation.y, bindingRotationTolerance )
					&& Helpers.AngleIsWithinTolerance( localRotation.z, bindingRotation.z, bindingRotationTolerance );
			}
		}

		void UpdateBindingAnimation ()
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
					if (neckLinker.stretched)
					{
						if (printEvents) { Debug.Log( name + " release while binding" ); }
						Release();
					}
					bindingAttractor.attractiveForce = bindingForce * (Time.time - bindStartTime) / bindTime;
				}
				else if (releasing)
				{
					bindingAttractor.attractiveForce = bindingForce * (1f - (Time.time - bindStartTime) / bindTime);
				}
			}
			else
			{
				bindingAttractor.Stop();
				if (binding)
				{
					rotator.SnapToGoal();
					body.isKinematic = true;
					body.position = tubulin.transform.TransformPoint( bindingPosition );
					binding = false;
					if (atpBinder.nucleotide != null && atpBinder.nucleotide.isATP)
					{
						BindATP();
					}
				}
				if (releasing)
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

		void UpdateCheckRelease ()
		{
			if (bound && !binding)
			{
				if (shouldRelease)
				{
					if (printEvents) { Debug.Log(name + " released w/ probability in state " + state.ToString()); }
					Release();
				}
			}
		}

		bool shouldRelease
		{
			get {
				float probability = (state == MotorState.Weak) ? probabilityOfEjectionFromWeak : probabilityOfEjectionFromStrong;
				return Random.Range(0, 1f) <= Time.deltaTime * probability;
			}
		}

		float probabilityOfEjectionFromWeak
		{
			get {
				float probability = kinesin.motorReleaseProbabilityMin;
				if (!inFront) // this is back motor
				{
					if (neckLinker.bound)
					{
						probability = (otherMotor.bound) ? kinesin.motorReleaseProbabilityMax : kinesin.motorReleaseProbabilityMin;
					}
					else
					{
						// p ~= min when tension < 0.5, p ~= max when tension > 0.8
						probability = kinesin.motorReleaseProbabilityMin + (kinesin.motorReleaseProbabilityMax - kinesin.motorReleaseProbabilityMin) 
							/ (1f + Mathf.Exp( -30f * (neckLinker.tension - 0.65f) ));
					}
				}
				return probability;
			}
		}

		float probabilityOfEjectionFromStrong
		{
			get {
				return 0;
			}
		}

		public void Release ()
		{
			if (bound)
			{
				rotator.rotating = false;
				neckLinker.Release();
				binding = false;
				releasing = true;
				body.constraints = RigidbodyConstraints.None;
				body.isKinematic = false;
				bindingAttractor.GoToTransform( tubulin.transform, bindingForce );
				bindStartTime = Time.time;
			}
		}

		void UpdateCheckNecklinker ()
		{
			if (shouldReleaseNecklinker && state != MotorState.Strong)
			{
				neckLinker.Release();
				shouldReleaseNecklinker = false;
			}
		}

		// ---------------------------------------------- Push Forward

		float pushForwardTime;

		Attractor _pushingAttractor;
		Attractor pushingAttractor
		{
			get {
				if (_pushingAttractor == null)
				{
					_pushingAttractor = gameObject.AddComponent<Attractor>();
				}
				return _pushingAttractor;
			}
		}

		public void PushForward ()
		{
			if (kinesin.pushOtherMotorForwardAfterSnap)
			{
				if (printEvents) { Debug.Log( name + " push forward" ); }
				pushingAttractor.GoToPosition( kinesin.hips.transform.position + 6f * otherMotor.transform.forward, 50f );
				pushForwardTime = Time.time;
			}
		}

		void UpdatePushForward ()
		{
			if (pushingAttractor.attracting && Time.time - pushForwardTime > 3f)
			{
				if (printEvents) { Debug.Log( name + " stop pushing" ); }
				pushingAttractor.Stop();
			}
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

		float lastATPTime = -1f;

		public void CollideWithATP ()
		{
			lastATPTime = Time.time;
		}

		public void BindATP ()
		{
			if (state == MotorState.Weak && !binding && !releasing)
			{
				if (printEvents) { Debug.Log( name + " SNAP" ); }
				state = MotorState.Strong;
				otherMotor.shouldReleaseNecklinker = true;
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
			// p ~= min when tension > 0.9, p ~= max when tension < 0.6
			atpBinder.ATPBindingProbability = kinesin.ATPBindProbabilityMin + (kinesin.ATPBindProbabilityMax - kinesin.ATPBindProbabilityMin) 
				* (1f - 1f / (1f + Mathf.Exp( -30f * (neckLinker.tension - 0.75f) )));
		}

		void UpdateADPReleaseProbability ()
		{
			float probability = kinesin.ADPReleaseProbabilityMax;
			if (!inFront) // this is the back motor
			{
				// p ~= min when tension > 0.9, p ~= max when tension < 0.6
				probability = kinesin.ADPReleaseProbabilityMin + (kinesin.ADPReleaseProbabilityMax - kinesin.ADPReleaseProbabilityMin) 
					* (1f - 1f / (1f + Mathf.Exp( -30f * (neckLinker.tension - 0.75f) )));
			}
			atpBinder.ADPReleaseProbability = probability;
		}

		public void HydrolyzeATP ()
		{
			neckLinker.StopSnapping();
			if (state == MotorState.Strong)
			{
				state = MotorState.Weak;
			}
		}
	}
}