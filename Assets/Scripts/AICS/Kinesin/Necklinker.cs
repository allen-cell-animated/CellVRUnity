using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Necklinker : MonoBehaviour 
	{
		public bool startDocked;
		public bool snapping;
		public bool bound;
		public float snappingForce = 0.1f;

		float startingLength;
		float retryTime;
		float retryWait = 0.2f;

		Link[] _links;
		public Link[] links
		{
			get {
				if (_links == null)
				{
					_links = GetComponentsInChildren<Link>();
				}
				return _links;
			}
		}

		public Link lastLink
		{
			get {
				return links[links.Length - 1];
			}
		}

		Motor _motor;
		public Motor motor
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
				Vector3 motorToHips = Vector3.Normalize( motor.kinesin.hips.transform.position - links[0].transform.position );
				float angle = Mathf.Acos( Vector3.Dot( motorToHips, motor.transform.forward ) );
				return angle < Mathf.PI / 2f;
			}
		}

		public float tension
		{
			get {
				float d = Vector3.Distance( links[0].transform.position, motor.kinesin.hips.transform.position );
				return (d - 0.5f) / 7f; // normalize between 0.5 and 7.5
			}
		}

		public bool stretched
		{
			get {
				return length / startingLength > 1.2f;
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
				links[i].dockedPosition = dockedPositions[i];
			}
		}

		void Start ()
		{
			startingLength = length;
		}

		// Unchanged vector components should be < 0
		public void SetJointRotationLimits (Vector3 newLimits)
		{
			foreach (Link link in links)
			{
				link.SetJointRotationLimits( newLimits );
			}
		}

		public void SetLinkMass (float mass)
		{
			foreach (Link link in links)
			{
				link.SetMass( mass );
			}
		}

		public void StartSnapping ()
		{
			if (!snapping)
			{
				snapping = bound = true;
				links[0].StartSnapping();
			}
		}

		public void FinishSnapping ()
		{
			if (motor.printEvents) { Debug.Log( motor.name + " finish snapping" ); }
			snapping = false;
			motor.otherMotor.PushForward();
		}

		public void StopSnapping ()
		{
			foreach (Link link in links)
			{
				link.StopSnapping();
			}
			snapping = false;
		}

		public void Release ()
		{
			foreach (Link link in links)
			{
				link.Release();
			}
			snapping = bound = false;
		}

		public void RetrySnapping ()
		{
			Release();
			Invoke( "StartSnapping", retryWait );
		}

		//for testing
		public float currentTension;

		void Update ()
		{
			if (snapping && stretched)
			{
				if (motor.printEvents) { Debug.Log( motor.name + " stopped snapping because stretched" ); }
				StopSnapping();
			}
			currentTension = tension;
			SetColor();
		}

		void SetColor ()
		{
			Color color = Color.black;
			if (!snapping)
			{
				float t = 0.4f * (1f - tension);
				color = Color.HSVToRGB( t, 1f, 1f );
			}

			foreach (Link link in links)
			{
				link.SetColor( color );
			}
		}
	}
}
