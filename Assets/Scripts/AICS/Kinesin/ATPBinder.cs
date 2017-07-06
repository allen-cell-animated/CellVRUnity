using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	[RequireComponent( typeof(IBindATP) )]
	public class ATPBinder : MonoBehaviour 
	{
		public Vector3 ATPBindingPosition = new Vector3( -0.66f, 0.73f, -2.5f );
		public float ATPBindingProbability = 1f;
		public float ADPReleaseProbability = 1f;
		public float hydrolysisTime = 5f;
		public Nucleotide nucleotide;

		float bindingTime = -1f;

		IBindATP _binder;
		IBindATP binder
		{
			get {
				if (_binder == null)
				{
					_binder = GetComponent<IBindATP>();
				}
				return _binder;
			}
		}

		void OnCollisionEnter (Collision collision)
		{
			Nucleotide _nucleotide = collision.collider.GetComponent<Nucleotide>();
			if (_nucleotide != null && _nucleotide.isATP) { binder.CollideWithATP(); } // for testing
			if (ShouldATPBind( _nucleotide ))
			{
				BindATP( _nucleotide );
			}
		}

		bool ShouldATPBind (Nucleotide _nucleotide)
		{
			if (nucleotide == null && _nucleotide != null && _nucleotide.isATP)
			{
				float random = Random.Range(0, 1f);
				return random <= ATPBindingProbability;
			}
			return false;
		}

		void BindATP (Nucleotide _nucleotide)
		{
			bindingTime = Time.time;
			_nucleotide.isBusy = true;
			Rigidbody body = _nucleotide.GetComponent<Rigidbody>();
			if (body != null) 
			{
				Destroy( body );
			}
			_nucleotide.transform.SetParent( transform );
			_nucleotide.transform.position = transform.TransformPoint( ATPBindingPosition );
			binder.BindATP();
			nucleotide = _nucleotide;
		}

		void Update ()
		{
			if (shouldReleaseADP)
			{
				ReleaseADP();
			}
			else if (shouldHydrolyzeATP)
			{
				HydrolyzeATP();
			}
		}

		bool shouldReleaseADP
		{
			get {
				if (nucleotide != null && !nucleotide.isATP && Time.time - bindingTime > 0.5f)
				{
					float random = Random.Range(0, 1f);
					return random <= Time.deltaTime * ADPReleaseProbability;
				}
				return false;
			}
		}

		void ReleaseADP ()
		{
			Nucleotide _nucleotide = nucleotide;
			nucleotide = null;
			if (!_nucleotide.GetComponent<Rigidbody>())
			{
				Rigidbody body = _nucleotide.gameObject.AddComponent<Rigidbody>();
				body.useGravity = false;
				body.mass = 0.1f;
				body.drag = 5f;
			}
			_nucleotide.transform.SetParent( _nucleotide.parent );
			_nucleotide.isBusy = false;
		}

		bool shouldHydrolyzeATP
		{
			get {
				if (nucleotide != null && nucleotide.isATP)
				{
					float probability = (Time.time - bindingTime) / hydrolysisTime - 0.5f;
					float random = Random.Range(0, 1f);
					return random <= probability;
				}
				return false;
			}
		}

		void HydrolyzeATP ()
		{
			nucleotide.Hydrolyze();
			binder.HydrolyzeATP();
		}
	}
}
