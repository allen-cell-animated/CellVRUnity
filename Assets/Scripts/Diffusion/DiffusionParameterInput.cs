using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Diffusion
{
	public class DiffusionParameterInput : MonoBehaviour 
	{
		public Parameter dTime; // = 100, 100 ps --> 1 μs
		public Parameter diffusionCoefficient; // = 20, 2 --> 80 μm²/s
		public float forceMultiplier = 2100f;
		public float torqueMultiplier = 1500f;

		static DiffusionParameterInput _Instance;
		public static DiffusionParameterInput Instance
		{
			get {
				if (_Instance == null)
				{
					_Instance = GameObject.FindObjectOfType<DiffusionParameterInput>();
				}
				return _Instance;
			}
		}

		public void SetDTime (float _sliderValue)
		{
			dTime.Set( _sliderValue );
		}

		public void SetDiffusionCoefficient (float _sliderValue)
		{
			diffusionCoefficient.Set( _sliderValue );
		}
	}
}