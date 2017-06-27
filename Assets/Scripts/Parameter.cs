﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AICS
{
	public enum ParameterFormat
	{
		Time,
		Round
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
		public int decimalPoints;

		public void Set (float _sliderValue) // slider goes from 0 --> 10
		{
			value = MapValue( _sliderValue );
			displayValue.text = FormatDisplay();
		}

		float MapValue (float _sliderValue)
		{
			switch (mapping) 
			{
				case SliderMapping.Linear :
					return MapValueLinear( _sliderValue );
				case SliderMapping.Logarithmic :
					return MapValueLogarithmic( _sliderValue );
				default :
					return 0;
			}
		}

		float MapValueLinear (float _sliderValue)
		{
			return min + _sliderValue / 10f * (max - min);
		}

		float MapValueLogarithmic (float _sliderValue)
		{
			return Mathf.Pow( 10f, Mathf.Log10( min ) + _sliderValue / 10f * (Mathf.Log10( max ) - Mathf.Log10( min )) );
		}

		string FormatDisplay ()
		{
			switch (format) 
			{
				case ParameterFormat.Time :
					return FormatTime();
				case ParameterFormat.Round :
					return FormatRound();
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
				units = "μs";
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

		string FormatRound ()
		{
			float multiplier = Mathf.Pow( 10f, decimalPoints );
			return (Mathf.Round( value * multiplier ) / multiplier) + " " + units;
		}
	}
}
