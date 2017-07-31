﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AICS.UI;

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
	public class RangeParameter : Parameter
	{
		public float rangeValue;
		public RangeSlider rangeSlider;

		public override void InitSlider ()
		{
			if (rangeSlider != null)
			{
				rangeSlider.minSlider.value = UnmapValue( value );
				rangeSlider.maxSlider.value = UnmapValue( rangeValue );
			}
		}

		public void SetMin (float _sliderValue)
		{
			value = MapValue( _sliderValue );
			SetDisplay();
		}

		public void SetMax (float _sliderValue)
		{
			rangeValue = MapValue( _sliderValue );
			SetDisplay();
		}

		void SetDisplay ()
		{
			displayValue.text = FormatDisplay( value ) + " - " + FormatDisplay( rangeValue );
		}
	}

	[System.Serializable]
	public class Parameter
	{
		public float value;
		public float max;
		public float min;
		public SliderMapping mapping;
		public Slider slider;
		public Text displayValue;
		public string units;
		public ParameterFormat format;
		public int decimalPoints;
		public bool spaceBeforeUnits = true;

		public void Set (float _sliderValue) // slider goes from 0 --> 10
		{
			value = MapValue( _sliderValue );
			displayValue.text = FormatDisplay( value );
		}

		protected float MapValue (float _sliderValue)
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
			return min + _sliderValue * (max - min);
		}

		float MapValueLogarithmic (float _sliderValue)
		{
			return Mathf.Pow( 10f, Mathf.Log10( min ) + _sliderValue * (Mathf.Log10( max ) - Mathf.Log10( min )) );
		}

		public virtual void InitSlider ()
		{
			if (slider != null)
			{
				slider.value = UnmapValue( value );
			}
		}

		protected float UnmapValue (float _value)
		{
			switch (mapping) 
			{
			case SliderMapping.Linear :
				return UnmapValueLinear( _value );
			case SliderMapping.Logarithmic :
				return UnmapValueLogarithmic( _value );
			default :
				return 0;
			}
		}

		float UnmapValueLinear (float _value)
		{
			return (_value - min) / (max - min);
		}

		float UnmapValueLogarithmic (float _value)
		{
			return (Mathf.Log10( _value ) - Mathf.Log10( min )) / (Mathf.Log10( max ) - Mathf.Log10( min ));
		}

		protected string FormatDisplay (float _value)
		{
			switch (format) 
			{
				case ParameterFormat.Time :
					return FormatTime( _value );
				case ParameterFormat.Round :
					return FormatRound( _value );
				default :
					return "";
			}
		}

		string space
		{
			get
			{
				return spaceBeforeUnits ? " " : "";
			}
		}

		string FormatTime (float _value)
		{
			string n = _value.ToString();
			if (_value >= 1000000f)
			{
				n = Mathf.Round( _value / 1000000f ).ToString();
				units = "μs";
			}
			else if (_value >= 1000f)
			{
				n = Mathf.Round( _value / 1000f ).ToString();
				units = "ns";
			}
			else
			{
				n = Mathf.Round( _value ).ToString();
				units = "ps";
			}
			return n + space + units;
		}

		string FormatRound (float _value)
		{
			float multiplier = Mathf.Pow( 10f, decimalPoints );
			string n = (Mathf.Round( _value * multiplier ) / multiplier).ToString();
			string[] splitN = n.Split('.');
			string wholeN = splitN[0];
			if (wholeN.Length > 3)
			{
				int d = wholeN.Length - 3;
				string result = wholeN.Substring( d, 3 );
				for (int i = wholeN.Length - 6; i > 0; i -= 3)
				{
					result = wholeN.Substring( i, 3 ) + "," + result;
					d = i;
				}
				result = wholeN.Substring( 0, d ) + "," + result;
				if (splitN.Length > 1)
				{
					n = result + "." + splitN[1];
				}
				else 
				{
					n = result;
				}
			}
			return n + space + units;
		}
	}
}