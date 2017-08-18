using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins
{
	[System.Serializable]
	public class Kinetic
	{
		public KineticRate kineticRate;
		public int attempts;
		public int events;
		public float observedKineticRate;

		public Kinetic (KineticRate _rate)
		{
			kineticRate = _rate;
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