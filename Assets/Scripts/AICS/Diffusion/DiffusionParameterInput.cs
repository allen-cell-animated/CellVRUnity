using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Diffusion
{
	public class DiffusionParameterInput : ParameterInput<DiffusionParameterInput> 
	{
		public Parameter diffusionCoefficient; // = 20, 2 --> 80 μm²/s
		public float forceMultiplier = 2100f;
		public float torqueMultiplier = 1500f;

		public void SetDiffusionCoefficient (float _sliderValue)
		{
			diffusionCoefficient.Set( _sliderValue );
		}
	}
}