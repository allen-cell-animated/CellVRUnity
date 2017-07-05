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

		public Rigidbody lastLink // used to setup hips
		{
			get {
				return links[links.Length - 1].GetComponent<Rigidbody>();
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

		public bool stretched
		{
			get {
				return tension > 1.3f;
			}
		}

		public float normalizedTension
		{
			get {
				return Mathf.Clamp( (tension - 0.9f) / 0.2f, 0, 1f );
			}
		}

		public float tension
		{
			get {
				return length / startingLength;
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
			snapping = false;
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
			Invoke( "StartSnapping", 0.2f );
		}

		//for testing
		public float currentTension;
		public bool currentTensionIsForward;
		void Update ()
		{
			currentTension = tension;
			currentTensionIsForward = tensionIsForward;
			SetColor();
		}

		void SetColor ()
		{
			float t = 0.4f + (tensionIsForward ? -1 : 1) * 0.4f * normalizedTension;
			Color color = Color.HSVToRGB( t, 1f, 1f );
			foreach (Link link in links)
			{
				link.SetColor( color );
			}
		}
	}
}
