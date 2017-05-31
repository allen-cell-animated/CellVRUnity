﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Kinesin : MonoBehaviour 
	{
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

		Motor[] _motors;
		public Motor[] motors
		{
			get {
				if (_motors == null)
				{
					_motors = GetComponentsInChildren<Motor>();
				}
				return _motors;
			}
		}

		RandomForces[] _randomForces;
		RandomForces[] randomForces
		{
			get {
				if (_randomForces == null)
				{
					_randomForces = GetComponentsInChildren<RandomForces>();
				}
				return _randomForces;
			}
		}

		public float tensionToRemoveWeaklyBoundMotor = 0.6f;
		public float maxTension = 0.8f;
		public float ATPHydrolysisTime = 1f;
		public Color ATPColor;
		public Color ADPColor;
		public float neckLinkerSnappingForce = 200;

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

		public void ToggleRandomForces (bool enabled)
		{
			foreach (RandomForces forces in randomForces)
			{
				forces.addForces = enabled;
			}
		}

		public void RandomlyReleaseStrongMotor ()
		{
			foreach (Motor motor in motors)
			{
				if (motor.state != MotorState.Strong)
				{
					return;
				}
			}
			motors[Random.Range(0, 2)].Release();
		}
	}
}
