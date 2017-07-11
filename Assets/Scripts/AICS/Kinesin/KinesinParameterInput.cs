using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AICS.Kinesin
{
	public class KinesinParameterInput : ParameterInput<KinesinParameterInput>  
	{
		Kinesin _kinesin;
		Kinesin kinesin
		{
			get {
				if (_kinesin == null)
				{
					_kinesin = GameObject.FindObjectOfType<Kinesin>();
				}
				return _kinesin;
			}
		}

		public RangeParameter ejectionProbability; // = (0.01, 0.9), 0.01 --> 0.99
		public RangeParameter ATPBindProbability; // = (0.01, 0.9), 0.01 --> 0.99
		public RangeParameter ADPReleaseProbability; // = (0.01, 0.9), 0.01 --> 0.99

		public Parameter hipsMass; // = 0.1, 0.01 --> 1
		public Parameter motorMass; // = 0.1, 0.01 --> 1
		public Parameter linkMass; // = 0.03, 0.01 --> 1

		public Toggle pushForwardToggle; // = true

		public Parameter motorBindingRotationTolerance; // = 180, 0 --> 180

		public Parameter hipRotationX; // = 0, 0 --> 180
		public Parameter hipRotationY; // = 0, 0 --> 180
		public Parameter hipRotationZ; // = 0, 0 --> 180

		public Parameter linkRotationX; // = 87, 0 --> 180
		public Parameter linkRotationY; // = 0, 0 --> 180
		public Parameter linkRotationZ; // = 5, 0 --> 180

		public override void InitSliders () 
		{
			ejectionProbability.InitSlider();
			ATPBindProbability.InitSlider();
			ADPReleaseProbability.InitSlider();
			hipsMass.InitSlider();
			motorMass.InitSlider();
			linkMass.InitSlider();
			motorBindingRotationTolerance.InitSlider();
			hipRotationX.InitSlider();
			hipRotationY.InitSlider();
			hipRotationZ.InitSlider();
			linkRotationX.InitSlider();
			linkRotationY.InitSlider();
			linkRotationZ.InitSlider();
		}

		public void SetEjectionProbabilityMin (float _sliderValue)
		{
			ejectionProbability.SetMin( _sliderValue );
			kinesin.ejectionProbabilityMin = ejectionProbability.value;
		}

		public void SetEjectionProbabilityMax (float _sliderValue)
		{
			ejectionProbability.SetMax( _sliderValue );
			kinesin.ejectionProbabilityMax = ejectionProbability.rangeValue;
		}

		public void SetATPBindProbabilityMin (float _sliderValue)
		{
			ATPBindProbability.SetMin( _sliderValue );
			kinesin.ATPBindProbabilityMin = ATPBindProbability.value;
		}

		public void SetATPBindProbabilityMax (float _sliderValue)
		{
			ATPBindProbability.SetMax( _sliderValue );
			kinesin.ATPBindProbabilityMax = ATPBindProbability.rangeValue;
		}

		public void SetADPReleaseProbabilityMin (float _sliderValue)
		{
			ADPReleaseProbability.SetMin( _sliderValue );
			kinesin.ADPReleaseProbabilityMin = ADPReleaseProbability.value;
		}

		public void SetADPReleaseProbabilityMax (float _sliderValue)
		{
			ADPReleaseProbability.SetMax( _sliderValue );
			kinesin.ADPReleaseProbabilityMax = ADPReleaseProbability.rangeValue;
		}

		public void SetMotorBindingRotationTolerance (float _sliderValue)
		{
			motorBindingRotationTolerance.Set( _sliderValue );
			kinesin.motorBindingRotationTolerance = motorBindingRotationTolerance.value;
		}

		public void SetHipRotationX (float _sliderValue)
		{
			hipRotationX.Set( _sliderValue );
			kinesin.SetHipsRotationLimits( new Vector3( hipRotationX.value, -1f, -1f ) );
		}

		public void SetHipRotationY (float _sliderValue)
		{
			hipRotationY.Set( _sliderValue );
			kinesin.SetHipsRotationLimits( new Vector3( -1f, hipRotationY.value, -1f ) );
		}

		public void SetHipRotationZ (float _sliderValue)
		{
			hipRotationZ.Set( _sliderValue );
			kinesin.SetHipsRotationLimits( new Vector3( -1f, -1f, hipRotationZ.value ) );
		}

		public void SetLinkRotationX (float _sliderValue)
		{
			linkRotationX.Set( _sliderValue );
			kinesin.SetLinkRotationLimits( new Vector3( linkRotationX.value, -1f, -1f ) );
		}

		public void SetLinkRotationY (float _sliderValue)
		{
			linkRotationY.Set( _sliderValue );
			kinesin.SetLinkRotationLimits( new Vector3( -1f, linkRotationY.value, -1f ) );
		}

		public void SetLinkRotationZ (float _sliderValue)
		{
			linkRotationZ.Set( _sliderValue );
			kinesin.SetLinkRotationLimits( new Vector3( -1f, -1f, linkRotationZ.value ) );
		}

		public void SetHipsMass (float _sliderValue)
		{
			hipsMass.Set( _sliderValue );
			kinesin.SetHipsMass( hipsMass.value );
		}

		public void SetMotorMass (float _sliderValue)
		{
			motorMass.Set( _sliderValue );
			kinesin.SetMotorMass( motorMass.value );
		}

		public void SetLinkMass (float _sliderValue)
		{
			linkMass.Set( _sliderValue );
			kinesin.SetLinkMass( linkMass.value );
		}

		public void SetPushForward ()
		{
			kinesin.pushOtherMotorForwardAfterSnap = pushForwardToggle.isOn;
		}
	}
}