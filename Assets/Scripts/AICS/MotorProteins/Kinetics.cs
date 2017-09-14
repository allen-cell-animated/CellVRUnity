using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins
{
	[System.Serializable]
	public class Kinetic
	{
		public bool log;
		public KineticRate kineticRate;
		public int attempts;
		public int events;
		public float observedKineticRate;

		public Kinetic (KineticRate _rate)
		{
			kineticRate = _rate;
		}

		public bool ShouldHappen (float nanosecondsPerStep, int stepsSinceStart)
		{
			string s = observedKineticRate + " / " + kineticRate.rate;
			if (observedRateTooLow)
			{
				if (log) { Debug.Log( "true " + s ); }
				return true;
			}
			if (observedRateTooHigh)
			{
				if (log) { Debug.Log( "false " + s ); }
				return false;
			}
			if (log) { Debug.Log( "try " + s ); }
			return Random.Range( 0, 1f ) <= kineticRate.rate * nanosecondsPerStep * 1E-9f * stepsSinceStart / attempts;
		}

		public bool observedRateTooHigh
		{
			get
			{
				return observedKineticRate > 1.2f * kineticRate.rate;
			}
		}

		public bool observedRateTooLow
		{
			get
			{
				return observedKineticRate < 0.8f * kineticRate.rate;
			}
		}
	}

	[System.Serializable]
	public class Kinetics 
	{
		public List<Kinetic> kinetics = new List<Kinetic>();

		public Kinetics (KineticRates kineticRates)
		{
			foreach (KineticRate r in kineticRates.rates)
			{
				kinetics.Add( new Kinetic( r ) );
			}
		}

		public void CalculateObservedRates (float nanosecondsSinceStart)
		{
			float secondsSinceStart = nanosecondsSinceStart * 1E-9f;
			foreach (Kinetic k in kinetics)
			{
				k.observedKineticRate = Mathf.Round( k.events / secondsSinceStart );
			}
		}

		public void Reset ()
		{
			foreach (Kinetic k in kinetics)
			{
				k.events = k.attempts = 0;
				k.observedKineticRate = 0;
			}
		}
	}
}