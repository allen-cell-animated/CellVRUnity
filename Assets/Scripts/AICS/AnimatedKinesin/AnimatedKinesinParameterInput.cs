using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AICS.AnimatedKinesin
{
	public class AnimatedKinesinParameterInput : ParameterInput<AnimatedKinesinParameterInput>  
	{
		public Kinesin kinesin;

		public Parameter ATPBindingProbabilityFront; // = 70%, 1 --> 99
		public Parameter ATPBindingProbabilityBack; // = 10%, 1 --> 99
		public Parameter ejectionProbabilityFront; // = 70%, 1 --> 99
		public Parameter ejectionProbabilityBack; // = 30%, 1 --> 99

		public RangeParameter necklinkerLength; // = (2, 6), 1 --> 9
		public Parameter snappingSpeed; // = 30°/s, 5 --> 100
		public Parameter meanStepSize; // = 0.2 nm, 0.05 --> 1

		public Parameter averageWalkingSpeed; // nm/s

		public override void InitSliders () 
		{
			ATPBindingProbabilityFront.InitSlider();
			ATPBindingProbabilityBack.InitSlider();
			ejectionProbabilityFront.InitSlider();
			ejectionProbabilityBack.InitSlider();
			necklinkerLength.InitSlider();
			snappingSpeed.InitSlider();
			meanStepSize.InitSlider();
		}

		public void SetATPBindingProbabilityFront (float _sliderValue)
		{
			ATPBindingProbabilityFront.Set( _sliderValue );
//			kinesin.ATPBindingProbabilityFront = ATPBindingProbabilityFront.value;
		}

		public void SetATPBindingProbabilityBack (float _sliderValue)
		{
			ATPBindingProbabilityBack.Set( _sliderValue );
//			kinesin.ATPBindingProbabilityBack = ATPBindingProbabilityBack.value;
		}

		public void SetEjectionProbabilityFront (float _sliderValue)
		{
			ejectionProbabilityFront.Set( _sliderValue );
//			kinesin.ejectionProbabilityFront = ejectionProbabilityFront.value;
		}

		public void SetEjectionProbabilityBack (float _sliderValue)
		{
			ejectionProbabilityBack.Set( _sliderValue );
//			kinesin.ejectionProbabilityBack = ejectionProbabilityBack.value;
		}

		public void SetNecklinkerLengthMin (float _sliderValue)
		{
			necklinkerLength.SetMin( _sliderValue );
			kinesin.SetNeckLinkerMinLength( necklinkerLength.value );
		}

		public void SetNecklinkerLengthMax (float _sliderValue)
		{
			necklinkerLength.SetMax( _sliderValue );
			kinesin.SetNeckLinkerMaxLength( necklinkerLength.rangeValue );
		}

		public void SetSnappingSpeed (float _sliderValue)
		{
			snappingSpeed.Set( _sliderValue );
			kinesin.hips.snapSpeed = snappingSpeed.value;
		}

		public void SetMeanStepSize (float _sliderValue)
		{
			meanStepSize.Set( _sliderValue );
			kinesin.SetMeanStepSize( meanStepSize.value );
		}

		public void Reset ()
		{
			kinesin.Reset();
		}

		public Text fpsDisplay;
		float lastTime = -1f;

		void Update ()
		{
			if (Time.time - lastTime > 0.3f)
			{
				fpsDisplay.text = Mathf.Round(1f / Time.deltaTime).ToString() + " fps";
				lastTime = Time.time;

				averageWalkingSpeed.SetDisplay( kinesin.averageWalkingSpeed );
			}
		}
	}
}