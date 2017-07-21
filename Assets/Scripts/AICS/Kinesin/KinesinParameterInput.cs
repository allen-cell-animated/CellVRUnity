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

		public Parameter motorMass; // = 40 kDa, 1 --> 100
		public Parameter linkMass; // = 0.15 kDa, 0.01 --> 5
		public Parameter hipsMass; // = 10 kDa, 1 --> 100
		public Parameter tropomyosinMass; // = 135 kDa, 10 --> 500
		public Parameter cargoMass; // = 15,000 kDa, 100 --> 50,000
		public Parameter ATPMass; // = 0.5 kDa, 0.01 --> 5

		public Toggle pushForwardToggle; // = true

		public Parameter motorBindingRotationTolerance; // = 180 degrees, 0 --> 180

		public Parameter hipRotationX; // = 0 degrees, 0 --> 180
		public Parameter hipRotationY; // = 0 degrees, 0 --> 180
		public Parameter hipRotationZ; // = 0 degrees, 0 --> 180

		public Parameter linkRotationX; // = 87 degrees, 0 --> 180
		public Parameter linkRotationY; // = 0 degrees, 0 --> 180
		public Parameter linkRotationZ; // = 5 degrees, 0 --> 180

		public override void InitSliders () 
		{
			ejectionProbability.InitSlider();
			ATPBindProbability.InitSlider();
			ADPReleaseProbability.InitSlider();
			motorMass.InitSlider();
			linkMass.InitSlider();
			hipsMass.InitSlider();
			tropomyosinMass.InitSlider();
			cargoMass.InitSlider();
			ATPMass.InitSlider();
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
			kinesin.hips.SetJointRotationLimits( new Vector3( hipRotationX.value, -1f, -1f ) );
		}

		public void SetHipRotationY (float _sliderValue)
		{
			hipRotationY.Set( _sliderValue );
			kinesin.hips.SetJointRotationLimits( new Vector3( -1f, hipRotationY.value, -1f ) );
		}

		public void SetHipRotationZ (float _sliderValue)
		{
			hipRotationZ.Set( _sliderValue );
			kinesin.hips.SetJointRotationLimits( new Vector3( -1f, -1f, hipRotationZ.value ) );
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

		public void SetHipsMass (float _sliderValue)
		{
			hipsMass.Set( _sliderValue );
			kinesin.hips.SetMass( hipsMass.value );
		}

		public void SetTropomyosinMass (float _sliderValue)
		{
			tropomyosinMass.Set( _sliderValue );
			kinesin.hips.SetTropomyosinMass( tropomyosinMass.value );
		}

		public void SetCargoMass (float _sliderValue)
		{
			cargoMass.Set( _sliderValue );
			kinesin.hips.SetCargoMass( cargoMass.value );
		}

		public void SetATPMass (float _sliderValue)
		{
			ATPMass.Set( _sliderValue );
			kinesin.hips.SetATPMass( ATPMass.value );
		}

		public void SetPushForward ()
		{
			kinesin.pushOtherMotorForwardAfterSnap = pushForwardToggle.isOn;
		}

		public Text fpsDisplay;
		float lastTime = -1f;

		void Update ()
		{
			if (Time.time - lastTime > 0.3f)
			{
				fpsDisplay.text = Mathf.Round(1f / Time.deltaTime).ToString() + " fps";
				lastTime = Time.time;
			}
		}
	}
}