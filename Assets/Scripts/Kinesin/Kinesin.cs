using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Kinesin : MonoBehaviour 
	{
		public float tensionToRemoveWeaklyBoundMotor = 0.2f;
		public float maxTension = 0.8f;
		public float neckLinkerSnappingForce = 25f;

		Hips _hips;
		public Hips hips
		{
			get {
				if (_hips == null)
				{
					_hips = GetComponentInChildren<Hips>();
				}
				return _hips;
			}
		}

		List<Motor> _motors;
		public List<Motor> motors
		{
			get {
				if (_motors == null)
				{
					_motors = new List<Motor>( GetComponentsInChildren<Motor>() );
				}
				return _motors;
			}
		}

		void Start ()
		{
			StoreDockedNecklinkPositions();
		}

		void StoreDockedNecklinkPositions ()
		{
			Necklinker[] necklinkers = GetComponentsInChildren<Necklinker>();
			Vector3[] dockedLinkPositions = new Vector3[necklinkers[0].links.Length];
			Quaternion[] dockedLinkRotations = new Quaternion[necklinkers[0].links.Length];
			foreach (Necklinker necklinker in necklinkers)
			{
				if (necklinker.startDocked)
				{
					Motor motor = necklinker.GetComponentInParent<Motor>();
					for (int i = 0; i < necklinker.links.Length; i++)
					{
						dockedLinkPositions[i] = motor.transform.InverseTransformPoint( necklinker.links[i].transform.position );
						dockedLinkRotations[i] = necklinker.links[i].transform.localRotation;
					}
				}
			}

			foreach (Necklinker necklinker in necklinkers)
			{
				necklinker.SetDockedLocations( dockedLinkPositions, dockedLinkRotations );
			}
		}

		public Motor OtherMotor (Motor motor)
		{
			return motors.Find( m => m != motor );
		}
	}
}
