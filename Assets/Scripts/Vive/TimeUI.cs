using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour 
{
	public Transform fill;
//	public Text text;
	[HideInInspector] public float minTimeMultiplier;
	[HideInInspector] public float maxTimeMultiplier;

	float fillMaxHeight = 1.2f;
	int significantFigures = 2;

	public void Set (float timeMultiplier) 
	{
		SetFill( timeMultiplier );
//		text.text = FormatNumber( timeMultiplier ) + "x";
	}

	void SetFill (float value)
	{
		float h = fillMaxHeight * (1 - (value - minTimeMultiplier) / (maxTimeMultiplier - minTimeMultiplier));
		fill.localPosition = new Vector3( -2.5f, -(fillMaxHeight - h) / 2f, 0 );
		fill.localScale = new Vector3( 1f, h, 1f );
	}

//	string FormatNumber (float value)
//	{
//		float logValue = Mathf.Log10( value );
//		float multiplier = Mathf.Pow( 10f, Mathf.Floor( logValue ) - significantFigures + 1 );
//		string n = "";
//		if (logValue < 0)
//		{
//			n = (Mathf.Round( value / multiplier ) / Mathf.Round( 1f / multiplier )).ToString();
//		}
//		else 
//		{
//			n = (Mathf.Round( value / multiplier ) * multiplier).ToString();
//		}
//
//		string[] splitN = n.Split('.');
//		string wholeN = splitN[0];
//		if (wholeN.Length > 3)
//		{
//			int d = wholeN.Length - 3;
//			string result = wholeN.Substring( d, 3 );
//			for (int i = wholeN.Length - 6; i > 0; i -= 3)
//			{
//				result = wholeN.Substring( i, 3 ) + "," + result;
//				d = i;
//			}
//			result = wholeN.Substring( 0, d ) + "," + result;
//			if (splitN.Length > 1)
//			{
//				n = result + "." + splitN[1];
//			}
//			else 
//			{
//				n = result;
//			}
//		}
//		return n;
//	}
}
