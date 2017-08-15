using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins
{
	[System.Serializable]
	public class KineticRate
	{
		public string id;
		public string description;
		public float rate; // s^-1

		public KineticRate (string _id, string _description, float _rate)
		{
			id = _id;
			description = _description;
			rate = _rate;
		}
	}

	public class KineticRates : ScriptableObject
	{
		public List<KineticRate> rates;

		public KineticRate GetRate (string id)
		{
			return rates.Find( r => r.id == id );
		}

		public void SetRate (string id, float rate)
		{
			rates.Find( r => r.id == id ).rate = rate;
		}

		public void SetKinesinDefaults () // from Pollard
		{
			rates = new List<KineticRate>();
			rates.Add( new KineticRate( "A", "ATP bind (Mt-K --> Mt-KT)", 1000f ) );
			rates.Add( new KineticRate( "B", "ATP unbind (Mt-KT --> Mt-K)", 100f ) );
			rates.Add( new KineticRate( "C", "Hydrolyze ATP (Mt-KT --> Mt-KDP)", 200f ) );
			rates.Add( new KineticRate( "D", "Phosphate unbind while bound to MT (Mt-KDP --> Mt-KD)", 80f ) );
			rates.Add( new KineticRate( "E", "Bind MT with ADP and Pi (KDP --> Mt-KDP)", 200f ) );
			rates.Add( new KineticRate( "F", "Unbind MT with ADP and Pi (Mt-KDP --> KDP)", 100f ) );
			rates.Add( new KineticRate( "G", "Phosphate unbind while free (KDP --> KD)", 10f ) );
			rates.Add( new KineticRate( "H", "Bind MT with ADP (KD --> Mt-KD)", 200f ) );
			rates.Add( new KineticRate( "I", "Unbind MT with ADP (Mt-KD --> KD)", 100f ) );
			rates.Add( new KineticRate( "J", "ADP unbind (Mt-KD --> Mt-K)", 300f ) );
		}
	}
}