using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Link
	{
		public Transform transform;
		public Rigidbody body;
		public RandomForces randomForces;
		public Vector3 dockedPosition;
		public Vector3 aboveDockedPosition;
		public bool readyToSnap;

		public Link (Rigidbody link)
		{
			transform = link.transform;
			body = link;
			randomForces = link.GetComponent<RandomForces>();
		}

		public void SetDockedPosition (Vector3 _dockedPosition)
		{
			dockedPosition = _dockedPosition;
			aboveDockedPosition = dockedPosition + Vector3.forward;
		}
	}

	public class Necklinker : MonoBehaviour 
	{
		public bool startDocked;
		float[] linkerTensionExtents = new float[]{1f, 8f};
		float startingLength;

		Link[] _links;
		public Link[] links
		{
			get {
				if (_links == null)
				{
					Rigidbody[] linkBodies = GetComponentsInChildren<Rigidbody>();
					_links = new Link[linkBodies.Length];
					for (int i = 0; i < _links.Length; i++)
					{
						_links[i] = new Link( linkBodies[i] );
					}
				}
				return _links;
			}
		}

		Motor _motor;
		Motor motor
		{
			get {
				if (_motor == null)
				{
					_motor = GetComponentInParent<Motor>();
				}
				return _motor;
			}
		}

		public bool tensionIsForward
		{
			get {
				Vector3 motorToHips = Vector3.Normalize( motor.kinesin.hips.transform.position - motor.transform.position );
				float angle = Mathf.Acos( Vector3.Dot( motorToHips, -motor.transform.right ) );
				return angle < Mathf.PI / 2f;
			}
		}

		public float tension
		{
			get {
				float linkerLength = Vector3.Distance( transform.position, motor.kinesin.hips.transform.position );
				return (linkerLength - linkerTensionExtents[0]) / (linkerTensionExtents[1] - linkerTensionExtents[0]);
			}
		}

		public bool stretched
		{
			get {
				return length > 1.2f * startingLength;
			}
		}

		float length
		{
			get {
				float length = 0;
				for (int i = 1; i < links.Length; i++)
				{
					length += Vector3.Distance( links[i].transform.position, links[i - 1].transform.position );
				}
				length += Vector3.Distance( links[links.Length - 1].transform.position, motor.kinesin.hips.transform.position );
				return length;
			}
		}

		public void SetDockedPositions (Vector3[] dockedPositions)
		{
			for (int i = 0; i < links.Length; i++)
			{
				links[i].SetDockedPosition( dockedPositions[i] );
			}
		}

		void Start ()
		{
			startingLength = length;
		}

		bool snapping;
		public int currentLink;

		public void StartSnapping ()
		{
			snapping = true;
			links[0].randomForces.enabled = false;
			FinishSnappingLink( 0 );
		}

		public void StopSnapping ()
		{
			if (snapping)
			{
				foreach (Link link in links)
				{
					link.body.constraints = RigidbodyConstraints.None;
					link.randomForces.enabled = true;
				}
				snapping = false;
			}
		}

		void Update ()
		{
			if (snapping)
			{
				SimulateSnapping();
			}
		}

		void StartSnappingLink (int index)
		{
			currentLink = index;
			links[currentLink].randomForces.enabled = false;
			links[currentLink].readyToSnap = false;
			readyToSnap = false;
			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.transform.position = motor.transform.TransformPoint( links[currentLink].dockedPosition );
			sphere.transform.localScale =  0.5f * Vector3.one;
		}

		public float distanceToGoal;
		public bool readyToSnap;

		void SimulateSnapping ()
		{
//			if (!links[currentLink].readyToSnap)
//			{
//				Vector3 toGoal = motor.transform.TransformPoint( links[currentLink].aboveDockedPosition ) - links[currentLink].transform.position;
//				distanceToGoal = Vector3.Magnitude( toGoal );
//				if (Vector3.Magnitude( toGoal ) > 0.5f)
//				{
//					links[currentLink].body.AddForce( motor.kinesin.neckLinkerSnappingForce * toGoal );
//				}
//				else
//				{
//					links[currentLink].readyToSnap = true;
//					readyToSnap = true;
//				}
//			}
//			else
//			{
				Vector3 toGoal = motor.transform.TransformPoint( links[currentLink].dockedPosition ) - links[currentLink].transform.position;
				distanceToGoal = Vector3.Magnitude( toGoal );
				if (Vector3.Magnitude( toGoal ) > 1f)
				{
					links[currentLink].body.AddForce( motor.kinesin.neckLinkerSnappingForce * toGoal );
				}
				else
				{
					FinishSnappingLink( currentLink );
				}
//			}
		}

		void FinishSnappingLink (int index)
		{
//			links[index].body.constraints = RigidbodyConstraints.FreezePosition;
			links[index].body.isKinematic = true;
			links[index].body.position = motor.transform.TransformPoint( links[index].dockedPosition );

			if (index < links.Length - 1)
			{
				StartSnappingLink( index + 1 );
			}
			else
			{
				snapping = false;
			}
		}
	}
}
