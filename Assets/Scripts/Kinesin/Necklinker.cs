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
		public Quaternion dockedRotation;

		public Link (Rigidbody link)
		{
			transform = link.transform;
			body = link;
			randomForces = link.GetComponent<RandomForces>();
		}

		public void SetDockedPosition (Vector3 _dockedPosition, Quaternion _dockedRotation)
		{
			dockedPosition = _dockedPosition;
			dockedRotation = _dockedRotation;
		}

		public void ToggleRandomForces (bool enable)
		{
			if (randomForces != null)
			{
				randomForces.enabled = enable;
			}
		}

		public void ToggleFreeze (bool freeze)
		{
			Debug.Log( transform.name );
			body.isKinematic = freeze; 
			if (freeze)
			{
				transform.localRotation = dockedRotation;
			}
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
						Debug.Log( linkBodies[i].name );
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

		public void SetDockedLocations (Vector3[] dockedPositions, Quaternion[] dockedRotations)
		{
			for (int i = 0; i < links.Length; i++)
			{
				links[i].SetDockedPosition( dockedPositions[i], dockedRotations[i] );
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
			links[0].ToggleRandomForces( false );
			FinishSnappingLink( 0 );
		}

		public void StopSnapping ()
		{
			if (snapping)
			{
				foreach (Link link in links)
				{
					link.ToggleRandomForces( true );
					link.ToggleFreeze( false );
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
			links[currentLink].ToggleRandomForces( false );
		}

		public float distanceToGoal; //testing

		void SimulateSnapping ()
		{
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
		}

		void FinishSnappingLink (int index)
		{
			if (index > 0) 
			{ 
				links[index - 1].ToggleFreeze( true );
			}

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
