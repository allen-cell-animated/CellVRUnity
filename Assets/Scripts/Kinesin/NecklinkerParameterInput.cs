using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AICS.Kinesin
{
	public class NecklinkerParameterInput : ParameterInput<NecklinkerParameterInput>  
	{
		public Parameter forceFrequency; // = 5, 1 s⁻¹ --> 30 s⁻¹
		public Parameter hipsMass; // = 0.1, 0.01 --> 1

		public Rigidbody hips;
		public float linkMass = 0.03f;
		public NecklinkerTest necklinker;
		public Text fps;

		float lastTime = -1f;
		float deltaTime;
		float frames;

		public void SetForceFrequency (float _sliderValue)
		{
			forceFrequency.Set( _sliderValue );
		}

		public void SetHipsMass (float _sliderValue)
		{
			hipsMass.Set( _sliderValue );
			hips.mass = linkMass * hipsMass.value;
		}

		public void Dock ()
		{
			necklinker.StartSnapping();
		}

		public void Release ()
		{
			necklinker.Release();
		}

		public void Reset ()
		{
			necklinker.Reset();
		}

		void Update ()
		{
			deltaTime += Time.deltaTime;
			frames++;
			if (Time.time - lastTime > 0.5f)
			{
				fps.text = Mathf.Round( frames / deltaTime ) + " fps";
				lastTime = Time.time;
				deltaTime = frames = 0;
			}
		}
	}
}