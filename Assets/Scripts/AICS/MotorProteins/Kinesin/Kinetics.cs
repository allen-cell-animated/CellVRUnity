using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins
{
	public delegate bool KineticEvent (Kinetic kinetic);

	[System.Serializable]
	public class Kinetic
	{
		public string name;
		public int startStateIndex;
		public int finalStateIndex;
		public KineticEvent kineticEvent;
		public int attempts;
		public int events;
		public float theoreticalRate;
		public float observedRate;
		public bool log; // for testing

		public bool observedRateTooHigh
		{
			get
			{
				return observedRate > 1.2f * theoreticalRate;
			}
		}

		public bool observedRateTooLow
		{
			get
			{
				return observedRate < 0.8f * theoreticalRate;
			}
		}

		public Kinetic (KineticRate _kineticRate)
		{
			name = _kineticRate.name;
			startStateIndex = _kineticRate.startStateIndex;
			finalStateIndex = _kineticRate.finalStateIndex;
			theoreticalRate = _kineticRate.rate;
		}

		public bool ShouldHappen ()
		{
			string s = observedRate + " / " + theoreticalRate;
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
			return Random.Range( 0, 1f ) <= theoreticalRate * MolecularEnvironment.Instance.nanosecondsPerStep * 1E-9f * MolecularEnvironment.Instance.stepsSinceStart / attempts;
		}

		public void CalculateObservedRate (float secondsSinceStart)
		{
			observedRate = Mathf.Round( events / secondsSinceStart );
		}

		public void Reset ()
		{
			events = attempts = 0;
			observedRate = 0;
		}
	}

	[System.Serializable]
	public class Kinetics
	{
		public List<Kinetic> kinetics = new List<Kinetic>();

		public Kinetics (KineticRates kineticRates)
		{
			foreach (KineticRate rate in kineticRates.rates)
			{
				kinetics.Add( new Kinetic( rate ) );
			}
		}

		public Kinetic GetKinetic (int startStateIndex, int finalStateIndex)
		{
			return kinetics.Find( k => k.startStateIndex == startStateIndex && k.finalStateIndex == finalStateIndex );
		}

		public void CalculateObservedRates ()
		{
			float secondsSinceStart = MolecularEnvironment.Instance.nanosecondsSinceStart * 1E-9f;
			foreach (Kinetic k in kinetics)
			{
				k.CalculateObservedRate( secondsSinceStart );
			}
		}

		public void Reset ()
		{
			foreach (Kinetic k in kinetics)
			{
				k.Reset();
			}
		}
	}
}