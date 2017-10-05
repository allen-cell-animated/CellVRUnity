using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins
{
	[System.Serializable]
	public class KineticRate
	{
		public string name;
		public int startStateIndex;
		public int finalStateIndex;
		public float rate; // s^-1

		public KineticRate (string _name, int _startStateIndex, int _finalStateIndex, float _rate)
		{
			name = _name;
			rate = _rate;
			startStateIndex = _startStateIndex;
			finalStateIndex = _finalStateIndex;
		}
	}

	public class KineticRates : ScriptableObject
	{
		public List<KineticRate> rates;
		
		public void SetKinesinDefaults () // from Pollard
		{
			rates = new List<KineticRate>();
			rates.Add( new KineticRate( "ATP bind", 0, 1, 1000f ) );
			rates.Add( new KineticRate( "ATP unbind", 1, 0, 100f ) );
			rates.Add( new KineticRate( "Hydrolyze ATP", 1, 2, 120f ) );
			rates.Add( new KineticRate( "Phosphate unbind while bound to MT", 2, 3, 80f ) );
			rates.Add( new KineticRate( "Bind MT with ADP and Pi", 4, 2, 200f ) );
			rates.Add( new KineticRate( "Unbind MT with ADP and Pi", 2, 4, 100f ) );
			rates.Add( new KineticRate( "Phosphate unbind while free", 4, 5, 10f ) );
			rates.Add( new KineticRate( "Bind MT with ADP", 5, 3, 200f ) );
			rates.Add( new KineticRate( "Unbind MT with ADP", 3, 5, 100f ) );
			rates.Add( new KineticRate( "ADP unbind", 3, 0, 300f ) );
		}
	}
}