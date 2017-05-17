using System.Collections;
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

		public float tensionToRemoveWeaklyBoundMotor = 0.6f;
		public float maxTension = 0.8f;
		public float atpHydrolysisTime = 1f;
		public Color atpColor;
		public Color adpColor;
	}
}
