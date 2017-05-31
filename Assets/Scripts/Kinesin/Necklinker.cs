using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Necklinker : MonoBehaviour 
	{
		public bool startDocked;
		public bool snapping;

		float[] linkerTensionExtents = new float[]{1f, 8f};
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

		public bool bindIsPhysicallyImpossible
		{
			get {
				return tension > motor.kinesin.maxTension || stretched;
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
				return length > 1.1f * startingLength;
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
				links[i].SetDockedTransform( dockedPositions[i], dockedRotations[i] );
			}
		}

		void Start ()
		{
			startingLength = length;
		}

		public void StartSnapping ()
		{
			snapping = true;
			motor.kinesin.ToggleRandomForces( false );
			links[0].StartSnapping();
		}

		public void FinishSnapping ()
		{
			snapping = false;
			motor.kinesin.ToggleRandomForces( true );
		}

		public void StopSnapping ()
		{
			foreach (Link link in links)
			{
				link.Release();
			}
			FinishSnapping();
		}

		// --------------------------------- Testing

		public bool forwardTension;
		public float stretchFactor;
		public float _tension;

		void Update ()
		{
			forwardTension = tensionIsForward;
			stretchFactor = length / startingLength;
			_tension = tension;
		}
	}
}
