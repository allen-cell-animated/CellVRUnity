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

		public void StartSnapping ()
		{
			snapping = true;
			randomForces.addForce = randomForces.addTorque = false;
			startSnappingTime = Time.time;
		}

		void Update ()
		{
			if (snapping)
			{
				SimulateSnapping();
			}
		}

		void SimulateSnapping ()
		{
			if (Time.time - startSnappingTime > 2f)
			{
				neckLinker.RetrySnapping();
			}
			Vector3 toGoal = neckLinker.motor.transform.TransformPoint( dockedPosition ) - transform.position;
			float distanceToGoal = Vector3.Magnitude( toGoal );
			if (distanceToGoal > 0.3f)
			{
				float snappingForce = KinesinParameterInput.Instance.dTime.value * neckLinker.snappingForce / (1f + Mathf.Exp( -10f * (distanceToGoal - 0.5f) ));
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
			snapping = false;
		}

		public void Release ()
		{
			snapping = false;
			body.isKinematic = false; 
			body.constraints = RigidbodyConstraints.None;
			randomForces.addForce = randomForces.addTorque = true;
		}
	}
}
