using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	[RequireComponent( typeof(Rigidbody), typeof(RandomForces) )]
	public class Link : MonoBehaviour 
	{
		public bool snapping;

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

		Vector3 dockedPosition;
		Quaternion dockedRotation;

		public void SetDockedTransform (Vector3 _dockedPosition, Quaternion _dockedRotation)
		{
			dockedPosition = _dockedPosition;
			dockedRotation = _dockedRotation;
		}

		public void StartSnapping ()
		{
			snapping = true;
		}

		void Update ()
		{
			if (snapping && !neckLinker.bindIsPhysicallyImpossible)
			{
				SimulateSnapping();
			}
		}

		public float distanceToGoal;

		void SimulateSnapping ()
		{
			Vector3 toGoal = neckLinker.motor.transform.TransformPoint( dockedPosition ) - transform.position;
			distanceToGoal = Vector3.Magnitude( toGoal );
			if (Vector3.Magnitude( toGoal ) > 0.5f)
			{
				body.AddForce( neckLinker.motor.kinesin.neckLinkerSnappingForce * Vector3.Normalize( toGoal ) );
			}
			else
			{
				FinishSnapping();
			}
		}

		void FinishSnapping ()
		{
			snapping = false;
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

		public void Freeze ()
		{
			body.isKinematic = true; 
			transform.position = neckLinker.motor.transform.TransformPoint( dockedPosition );
			transform.localRotation = dockedRotation;
		}

		public void Release ()
		{
			snapping = false;
			body.isKinematic = false; 
		}
	}
}
