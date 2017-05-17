using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Necklinker : MonoBehaviour 
	{
		float[] linkerTensionExtents = new float[]{1f, 8f};
		float startingLength;

		Transform[] _links;
		Transform[] links
		{
			get {
				if (_links == null)
				{
					_links = GetComponentsInChildren<Transform>();
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

		void Start ()
		{
			startingLength = length;
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
					length += Vector3.Distance( links[i].position, links[i - 1].position );
				}
				length += Vector3.Distance( links[links.Length - 1].position, motor.kinesin.hips.transform.position );
				return length;
			}
		}
	}
}
