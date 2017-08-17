using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins
{
	[System.Serializable]
	public class KineticRate
	{
		public string description;
		public float rate; // s^-1

		public KineticRate (string _description, float _rate)
		{
			description = _description;
			rate = _rate;
		}
	}

	public class KineticRates : ScriptableObject
	{
		public List<KineticRate> rates;

		public void SetKinesinDefaults () // from Pollard
		{
			rates = new List<KineticRate>();
			rates.Add( new KineticRate( "ATP bind (Mt-K --> Mt-KT)", 1000f ) );
			rates.Add( new KineticRate( "ATP unbind (Mt-KT --> Mt-K)", 100f ) );
			rates.Add( new KineticRate( "Hydrolyze ATP (Mt-KT --> Mt-KDP)", 200f ) );
			rates.Add( new KineticRate( "Phosphate unbind while bound to MT (Mt-KDP --> Mt-KD)", 80f ) );
			rates.Add( new KineticRate( "Bind MT with ADP and Pi (KDP --> Mt-KDP)", 200f ) );
			rates.Add( new KineticRate( "Unbind MT with ADP and Pi (Mt-KDP --> KDP)", 100f ) );
			rates.Add( new KineticRate( "Phosphate unbind while free (KDP --> KD)", 10f ) );
			rates.Add( new KineticRate( "Bind MT with ADP (KD --> Mt-KD)", 200f ) );
			rates.Add( new KineticRate( "Unbind MT with ADP (Mt-KD --> KD)", 100f ) );
			rates.Add( new KineticRate( "ADP unbind (Mt-KD --> Mt-K)", 300f ) );
		}
	}
}