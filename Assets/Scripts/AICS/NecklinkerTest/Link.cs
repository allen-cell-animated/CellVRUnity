using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Necklinker
{
	[RequireComponent( typeof(Rigidbody), typeof(RandomForces) )]
	public class Link : MonoBehaviour 
	{
		public bool snapping;
		public Vector3 dockedPosition;

		bool startedSnapping;
		Transform startTransform;
		float startSnappingTime;

		Necklinker _necklinker;
		public Necklinker neckLinker
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

		CharacterJoint _joint;
		public CharacterJoint joint
		{
			get {
				if (_joint == null)
				{
					_joint = GetComponent<CharacterJoint>();
				}
				return _joint;
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

		MeshRenderer _meshRenderer;
		MeshRenderer meshRenderer
		{
			get {
				if (_meshRenderer == null)
				{
					_meshRenderer = transform.FindChild( "Sphere" ).GetComponent<MeshRenderer>();
				}
				return _meshRenderer;
			}
		}

		void Start ()
		{
			startTransform = new GameObject( "start_" + name ).transform;
			startTransform.position = transform.position;
		}

		public void StartSnapping ()
		{
			attractor.Stop();
			snapping = startedSnapping = true;
			randomForces.addForce = randomForces.addTorque = false;
			startSnappingTime = Time.time;
		}

		void Update ()
		{
			if (snapping)
			{
				SimulateSnapping();
			}

			SetColor();
		}

		void SetColor ()
		{
			if (snapping)
			{
				meshRenderer.material.color = Color.green;
			}
			else if (body.isKinematic)
			{
				meshRenderer.material.color = Color.blue;
			}
			else if (startedSnapping)
			{
				meshRenderer.material.color = Color.cyan;
			}
			else
			{
				meshRenderer.material.color = Color.white;
			}
		}

		public float distanceToGoal; //for testing
		public float snappingForce;

		void SimulateSnapping ()
		{
			if (Time.time - startSnappingTime > 2f)
			{
				neckLinker.RetrySnapping();
			}
			Vector3 toGoal = neckLinker.motor.transform.TransformPoint( dockedPosition ) - transform.position;
			distanceToGoal = Vector3.Magnitude( toGoal );
			if (distanceToGoal > 0.3f)
			{
				snappingForce = NecklinkerParameterInput.Instance.dTime.value * neckLinker.motor.necklinkerSnappingForce / (1f + Mathf.Exp( -10f * (distanceToGoal - 0.5f) ));
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

			transform.position = neckLinker.motor.transform.TransformPoint( dockedPosition );
			body.constraints = RigidbodyConstraints.FreezePosition;
			distanceToGoal = Vector3.Magnitude( neckLinker.motor.transform.TransformPoint( dockedPosition ) - transform.position );

			if (previousLink != null) 
			{
				previousLink.Freeze();
			}

			if (nextLink != null)
			{
				nextLink.StartSnapping();
			}
			else
			{
				Freeze();
				neckLinker.FinishSnapping();
			}
		}

		void Freeze ()
		{
			body.isKinematic = true; 
			transform.position = neckLinker.motor.transform.TransformPoint( dockedPosition );
		}

		public void StopSnapping ()
		{
			snapping = startedSnapping = false;
		}

		public void Release ()
		{
			attractor.Stop();
			snapping = startedSnapping = false;
			body.isKinematic = false; 
			body.constraints = RigidbodyConstraints.None;
			randomForces.addForce = randomForces.addTorque = true;
		}

		public void ResetPosition ()
		{
			if (startedSnapping)
			{
				Release();
			}
			attractor.GoToTransform( startTransform, 10f );
		}
	}
}
