using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class KinesinParameterInput : MonoBehaviour 
	{
		public Parameter dTime; // = 100, 100 ps --> 1 μs
		public Parameter diffusionCoefficient; // = 20, 2 --> 80 μm²/s

		static KinesinParameterInput _Instance;
		public static KinesinParameterInput Instance
		{
			get {
				if (_Instance == null)
				{
					_Instance = GameObject.FindObjectOfType<KinesinParameterInput>();
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