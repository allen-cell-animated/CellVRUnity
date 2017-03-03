using UnityEngine;
using System.Collections;

public class ShaderAdjuster : MonoBehaviour 
{
	public Material material;

	public void OnBrightnessSliderChange (float brightness)
	{
		material.SetFloat("_Brightness", brightness);
	}

	public void OnFalloffSliderChange (float falloff)
	{
		material.SetFloat("_Falloff", falloff);
	}

	public void OnThresholdSliderChange (float threshold)
	{
		material.SetFloat("_Threshold", -threshold);
	}
}
