using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	[RequireComponent( typeof(Rigidbody), typeof(RandomForces) )]
	public class Link : MonoBehaviour 
	{
		public bool snapping;
		public Vector3 dockedPosition;

		float startDistanceToHips;
		float startSnappingTime;
		float startDistanceToAnchor;
		int retries = 0;

		public float jointStretch 
		{
			get
			{
				return Vector3.Distance( joint.connectedBody.transform.position, transform.position ) / startDistanceToAnchor;
			}
		}

		public float stretchToHips
		{
			get
			{
				return Vector3.Distance( neckLinker.motor.kinesin.hips.transform.position, transform.position ) / startDistanceToHips;
			}
		}

		Necklinker _necklinker;
		Necklinker neckLinker
		{
			get {
				if (_necklinker == null)
				{
					_necklinker = GetComponentInParent<Necklinker>();
				}
				return _necklinker;
			}
		}

		Link _previousLink;
		Link previousLink
		{
			get {
				if (_previousLink == null)
				{
					_previousLink = transform.parent.GetComponent<Link>();
				}
				return _previousLink;
			}
		}

		Link _nextLink;
		Link nextLink
		{
			get {
				if (_nextLink == null)
				{
					_nextLink = transform.GetChild(0).GetComponent<Link>();
				}
				return _nextLink;
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

		ConfigurableJoint _joint;
		ConfigurableJoint joint
		{
			get {
				if (_joint == null)
				{
					_joint = GetComponent<ConfigurableJoint>();
				}
				return _joint;
			}
		}

		MeshRenderer _meshRenderer;
		MeshRenderer meshRenderer // testing
		{
			get {
				if (_meshRenderer == null)
				{
					_meshRenderer = transform.GetChild( 1 ).GetComponent<MeshRenderer>();
				}
				return _meshRenderer;
			}
		}

		void Start ()
		{
			startDistanceToAnchor = Vector3.Distance( joint.connectedBody.transform.position, transform.position );
			startDistanceToHips = Vector3.Distance( neckLinker.motor.kinesin.hips.transform.position, transform.position );
		}

		public void SetColor ()
		{
			Color color = Color.black;
			if (!snapping)
			{
				color = Color.HSVToRGB( 0.4f * (1f - Mathf.Clamp( (jointStretch - 0.9f) / 1.1f, 0, 1f )), 1f, 1f );
			}
			meshRenderer.material.color = color;
		}

		public void StartSnapping (int _retries)
		{
			if (_retries > 0 && previousLink != null)
			{
				Release();
				previousLink.StartSnapping( _retries - 1 );
			}
			else 
			{
				snapping = true;
				randomForces.addForce = randomForces.addTorque = false;
				startSnappingTime = Time.time;

				Unfreeze();
			}
		}

		void Update ()
		{
			if (snapping)
			{
				SimulateSnapping();
			}
			SetColor();
		}

		void SimulateSnapping ()
		{
			if (previousLink != null && Time.time - startSnappingTime > 0.5f)
			{
				Retry();
			}

			Vector3 toGoal = neckLinker.motor.transform.TransformPoint( dockedPosition ) - transform.position;
			float distanceToGoal = Vector3.Magnitude( toGoal );
			if (distanceToGoal > 0.3f)
			{
				float snappingForce = KinesinParameterInput.Instance.dTime.value * body.mass * neckLinker.snappingForce / (1f + Mathf.Exp( -10f * (distanceToGoal - 0.5f) ));
				body.AddForce( snappingForce * Vector3.Normalize( toGoal ) );
			}
			else
			{
				FinishSnapping();
			}
		}

		void FinishSnapping ()
		{
			snapping = false;
			retries = 0;

			Freeze();

			if (nextLink != null)
			{
				nextLink.StartSnapping( 0 );
			}
			else
			{
				Lock();
				neckLinker.FinishSnapping();
			}
		}

		void Retry ()
		{
			Release();
			previousLink.StartSnapping( retries );
			retries++;
		}

		void Unfreeze ()
		{
			body.constraints = RigidbodyConstraints.None;
			if (previousLink != null) 
			{
				previousLink.Unlock();
			}
		}

		void Freeze ()
		{
			transform.position = neckLinker.motor.transform.TransformPoint( dockedPosition );
			body.constraints = RigidbodyConstraints.FreezePosition;
			if (previousLink != null) 
			{
				previousLink.Lock();
			}
		}

		void Unlock ()
		{
			body.isKinematic = false; 
		}

		void Lock ()
		{
			body.isKinematic = true; 
			transform.position = neckLinker.motor.transform.TransformPoint( dockedPosition );
		}

		public void StopSnapping ()
		{
			snapping = false;
		}

		public void Release ()
		{
			snapping = false;
			body.isKinematic = false; 
			body.constraints = RigidbodyConstraints.None;
			randomForces.addForce = randomForces.addTorque = true;
		}

		public void SetMass (float mass)
		{
			body.mass = mass;
		}

		public void SetJointRotationLimits (Vector3 newLimits)
		{
			Helpers.SetJointRotationLimits( joint, newLimits );
		}
	}
}
