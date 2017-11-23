using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour 
{
	public RectTransform fill;
	public Text text;
	public float minTimeMultiplier;
	public float maxTimeMultiplier;

	float fillWidth = 0.3f;
	float fillMaxHeight = 1.7f;
	int significantFigures = 2;

	public void Set (float timeMultiplier) 
	{
		SetFill( timeMultiplier );
		text.text = FormatNumber( timeMultiplier ) + "x";
	}

	void SetFill (float value)
	{
		float h = fillMaxHeight * (value - minTimeMultiplier) / (maxTimeMultiplier - minTimeMultiplier);
		fill.rect.Set( 0, -h / 2f, fillWidth, h );
	}

	string FormatNumber (float value)
	{
		float logValue = Mathf.Log10( value );
		float multiplier = Mathf.Pow( 10f, Mathf.Floor( logValue ) - significantFigures + 1 );
		string n = "";
		if (logValue < 0)
		{
			n = (Mathf.Round( value / multiplier ) / Mathf.Round( 1f / multiplier )).ToString();
		}
		else 
		{
			n = (Mathf.Round( value / multiplier ) * multiplier).ToString();
		}

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
		return n;
	}
}
