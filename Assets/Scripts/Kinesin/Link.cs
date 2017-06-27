using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	[RequireComponent( typeof(Rigidbody), typeof(RandomForces) )]
	public class Link : MonoBehaviour 
	{
		public bool snapping;
		public float distanceToAnchor;

		float startDistanceToAnchor;
		public Vector3 dockedPosition;

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

		void Start ()
		{
			startDistanceToAnchor = Vector3.Distance( joint.connectedBody.transform.position, transform.position );
		}

		public void StartSnapping ()
		{
			snapping = startedSnapping = true;
		}
		bool startedSnapping;
		void Update ()
		{
			if (snapping) // && !neckLinker.stretched)
			{
				SimulateSnapping();
			}

			distanceToAnchor = Vector3.Distance( joint.connectedBody.transform.position, transform.position ) / startDistanceToAnchor;
//			if (distanceToAnchor > 5f)
//			{
//				if (neckLinker.motor.bound) { Debug.Log( "link released " + neckLinker.motor.name + " from tension " + distanceToAnchor); }
//				neckLinker.motor.ReleaseFromTension( name );
//			}
		}

		public float distanceToGoal; //for testing

		void SimulateSnapping ()
		{
			Vector3 toGoal = neckLinker.motor.transform.TransformPoint( dockedPosition ) - transform.position;
			distanceToGoal = Vector3.Magnitude( neckLinker.motor.transform.TransformPoint( dockedPosition ) - transform.position );
			if (distanceToGoal > 0.15f)
			{
				body.AddForce( neckLinker.snappingForce * toGoal );
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
				neckLinker.snapping = false;
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
		}
	}
}
