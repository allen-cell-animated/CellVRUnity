using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	[RequireComponent( typeof(Rigidbody), typeof(RandomForces) )]
	public class LinkTest : MonoBehaviour 
	{
		public bool snapping;
		public Vector3 dockedPosition;

		bool startedSnapping;

		NecklinkerTest _necklinker;
		public NecklinkerTest neckLinker
		{
			get {
				if (_necklinker == null)
				{
					_necklinker = GetComponentInParent<NecklinkerTest>();
				}
				return _necklinker;
			}
		}

		LinkTest _previousLink;
		LinkTest previousLink
		{
			get {
				if (_previousLink == null)
				{
					_previousLink = transform.parent.GetComponent<LinkTest>();
				}
				return _previousLink;
			}
		}

		LinkTest _nextLink;
		LinkTest nextLink
		{
			get {
				if (_nextLink == null)
				{
					_nextLink = transform.GetChild(0).GetComponent<LinkTest>();
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

		MeshRenderer _meshRenderer;
		MeshRenderer meshRenderer // testing
		{
			get {
				if (_meshRenderer == null)
				{
					_meshRenderer = transform.FindChild( "Sphere" ).GetComponent<MeshRenderer>();
				}
				return _meshRenderer;
			}
		}

		public void StartSnapping ()
		{
			snapping = startedSnapping = true;
			randomForces.addForce = randomForces.addTorque = false;
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

		void SimulateSnapping ()
		{
			Vector3 toGoal = neckLinker.motor.transform.TransformPoint( dockedPosition ) - transform.position;
			distanceToGoal = Vector3.Magnitude( toGoal );
			if (distanceToGoal > 0.3f)
			{
				neckLinker.motor.currentSnappingForce = neckLinker.motor.necklinkerSnappingForce / (1f + Mathf.Exp( -10f * (distanceToGoal - 0.5f) ));
				body.AddForce( neckLinker.motor.necklinkerSnappingForce / (1f + Mathf.Exp( -10f * (distanceToGoal - 0.5f) )) * Vector3.Normalize( toGoal ) 
					+ Helpers.GetRandomVector( neckLinker.motor.randomSnappingForce ));
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
			snapping = startedSnapping = false;
			body.isKinematic = false; 
			body.constraints = RigidbodyConstraints.None;
			randomForces.addForce = randomForces.addTorque = true;
		}
	}
}
