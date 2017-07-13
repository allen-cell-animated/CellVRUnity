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
		public float currentTension; // for testing

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
				foreach (Link link in links)
				{
					if (link.jointStretch > 2f)
					{
						return true;
					}
				}
				return false;
			}
		}

		public void SetDockedPositions (Vector3[] dockedPositions)
		{
			for (int i = 0; i < links.Length; i++)
			{
				links[i].dockedPosition = dockedPositions[i];
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

		void Update ()
		{
			if (snapping && stretched)
			{
				if (motor.printEvents) { Debug.Log( motor.name + " stopped snapping because stretched" ); }
				StopSnapping();
			}
			currentTension = tension;
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
	}
}
