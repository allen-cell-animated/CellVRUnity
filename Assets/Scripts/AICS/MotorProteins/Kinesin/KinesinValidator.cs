using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins.Kinesin
{
	[System.Serializable]
	public class RateValidator
	{
		public KineticRate kineticRate;
		public int events;
		public float observedRate;

		public RateValidator (KineticRate _rate)
		{
			kineticRate = _rate;
		}
	}

	public class KinesinValidator : MonoBehaviour 
	{
		public List<RateValidator> rates;

		public void Init (KineticRates kineticRates)
		{
			rates = new List<RateValidator>();
			foreach (KineticRate r in kineticRates.rates)
			{
				rates.Add( new RateValidator( r ) );
			}
		}

		public void IncrementEvents (int index)
		{
			if (index < rates.Count)
			{
				rates[index].events++;
			}
		}

		public void CalculateObservedRates (float nanosecondsSinceStart)
		{
			float secondsSinceStart = nanosecondsSinceStart * 1E-9f;
			foreach (RateValidator r in rates)
			{
				r.observedRate = Mathf.Round( r.events / secondsSinceStart );
			}
		}

		public void Reset ()
		{
			foreach (RateValidator r in rates)
			{
				r.events = 0;
			}
		}
	}
}