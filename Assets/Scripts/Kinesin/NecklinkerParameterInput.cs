using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class NecklinkerParameterInput : ParameterInput<NecklinkerParameterInput>  
	{
		public Parameter forceFrequency; // = 5, 1 s⁻¹ --> 30 s⁻¹
		public Parameter hipsMass; // = 0.1, 0.01 --> 1

		public Rigidbody hips;
		public NecklinkerTest necklinker;

		public void SetForceFrequency (float _sliderValue)
		{
			forceFrequency.Set( _sliderValue );
		}

		public void SetHipsMass (float _sliderValue)
		{
			hipsMass.Set( _sliderValue );
			hips.mass = hipsMass.value;
		}

		public void Dock ()
		{
			necklinker.StartSnapping();
		}

		public void Release ()
		{
			necklinker.Release();
		}

		public void Reset ()
		{
			necklinker.Reset();
		}
	}
}