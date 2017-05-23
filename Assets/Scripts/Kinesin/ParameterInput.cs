using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AICS.Kinesin
{
	public enum ParameterFormat
	{
		Time,
		RoundTo4Digits
	}

	public enum SliderMapping
	{
		Linear,
		Logarithmic
	}

	[System.Serializable]
	public class Parameter
	{
		public string name;
		public float value;
		public float max;
		public float min;
		public SliderMapping mapping;
		public Text displayValue;
		public string units;
		public ParameterFormat format;

		public void Set (float _sliderValue) // slider goes from 0 --> 10
		{
			value = SetValue( _sliderValue );
			displayValue.text = FormatDisplay();
		}

		float SetValue (float _sliderValue)
		{
			switch (mapping) 
			{
				case SliderMapping.Linear :
					return SetLinearValue( _sliderValue );
				case SliderMapping.Logarithmic :
					return SetLogarithmicValue( _sliderValue );
				default :
					return 0;
			}
		}

		float SetLinearValue (float _sliderValue)
		{
			return min + _sliderValue / 10f * (max - min);
		}

		float SetLogarithmicValue (float _sliderValue)
		{
			return Mathf.Pow( 10f, Mathf.Log10( min ) + _sliderValue / 10f * (Mathf.Log10( max ) - Mathf.Log10( min )) );
		}

		string FormatDisplay ()
		{
			switch (format) 
			{
				case ParameterFormat.Time :
					return FormatTime();
				case ParameterFormat.RoundTo4Digits :
					return FormatRoundTo4Digits();
				default :
					return "";
			}
		}

		string FormatTime ()
		{
			string n = value.ToString();
			if (value >= 1000000f)
			{
				n = Mathf.Round( value / 1000000f ).ToString();
				units = '\u03BC' + "s";
			}
			else if (value >= 1000f)
			{
				n = Mathf.Round( value / 1000f ).ToString();
				units = "ns";
			}
			else
			{
				n = Mathf.Round( value ).ToString();
				units = "ps";
			}
			return n + " " + units;
		}

		string FormatRoundTo4Digits ()
		{
			string n = (Mathf.Round( 10000f * value ) / 10000f).ToString();
			while (n.Length < 6)
			{
				n += "0";
			}
			return n + " " + units;
		}
	}

	public class ParameterInput : MonoBehaviour 
	{
		public Parameter dTime; // = 100f, 100 ps --> 1 us
		public Parameter diffusionCoefficient; // = 0.005f, 0.0002 --> 0.0140 A2/ps

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
