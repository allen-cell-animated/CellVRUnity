using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AICS.MotorProteins.Kinesin
{
	public class KinesinParameterInput : ParameterInput<KinesinParameterInput>  
	{
		public Kinesin kinesin;

		public Parameter[] rates;

		public Parameter timePerStep; // = 100 μs, 10 ns --> 1 ms
		public Parameter stepsPerFrame; // = 1, 1 --> 100
		public RangeParameter necklinkerLength; // = (1, 5), 1 --> 9
		public Parameter snappingSpeed; // = 90°/s, 5 --> 100
		public Parameter meanStepSize; // = 0.8 nm, 0.1 --> 2

		public Parameter averageWalkingSpeed; // μm/s

		public override void InitSliders () 
		{
			foreach (Parameter rate in rates)
			{
				rate.InitSlider();
			}
			timePerStep.InitSlider();
			stepsPerFrame.InitSlider();
			necklinkerLength.InitSlider();
			snappingSpeed.InitSlider();
			meanStepSize.InitSlider();
		}

		public void SetRateA (float _sliderValue)
		{
			rates[0].Set( _sliderValue );
			kinesin.kineticRates.rates[0].rate = rates[0].value;
		}

		public void SetRateB (float _sliderValue)
		{
			rates[1].Set( _sliderValue );
			kinesin.kineticRates.rates[1].rate = rates[1].value;
		}

		public void SetRateC (float _sliderValue)
		{
			rates[2].Set( _sliderValue );
			kinesin.kineticRates.rates[2].rate = rates[2].value;
		}

		public void SetRateD (float _sliderValue)
		{
			rates[3].Set( _sliderValue );
			kinesin.kineticRates.rates[3].rate = rates[3].value;
		}

		public void SetRateE (float _sliderValue)
		{
			rates[4].Set( _sliderValue );
			kinesin.kineticRates.rates[4].rate = rates[4].value;
		}

		public void SetRateF (float _sliderValue)
		{
			rates[5].Set( _sliderValue );
			kinesin.kineticRates.rates[5].rate = rates[5].value;
		}

		public void SetRateG (float _sliderValue)
		{
			rates[6].Set( _sliderValue );
			kinesin.kineticRates.rates[6].rate = rates[6].value;
		}

		public void SetRateH (float _sliderValue)
		{
			rates[7].Set( _sliderValue );
			kinesin.kineticRates.rates[7].rate = rates[7].value;
		}

		public void SetRateI (float _sliderValue)
		{
			rates[8].Set( _sliderValue );
			kinesin.kineticRates.rates[8].rate = rates[8].value;
		}

		public void SetRateJ (float _sliderValue)
		{
			rates[9].Set( _sliderValue );
			kinesin.kineticRates.rates[9].rate = rates[9].value;
		}

		public void SetTimePerStep (float _sliderValue)
		{
			timePerStep.Set( _sliderValue );
			MolecularEnvironment.Instance.nanosecondsPerStep = timePerStep.value * 1E-3f;
		}

		public void SetStepsPerFrame (float _sliderValue)
		{
			stepsPerFrame.Set( _sliderValue );
			MolecularEnvironment.Instance.stepsPerFrame = Mathf.RoundToInt( stepsPerFrame.value );
			MolecularEnvironment.Instance.SetCollisionDetectionMethod();
		}

		public void SetNecklinkerLengthMin (float _sliderValue)
		{
			necklinkerLength.SetMin( _sliderValue );
			kinesin.SetMinDistanceFromParent( necklinkerLength.value );
		}

		public void SetNecklinkerLengthMax (float _sliderValue)
		{
			necklinkerLength.SetMax( _sliderValue );
			kinesin.SetMaxDistanceFromParent( necklinkerLength.rangeValue );
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