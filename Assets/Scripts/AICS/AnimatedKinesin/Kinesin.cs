using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AnimatedKinesin
{
	public class Kinesin : MonoBehaviour 
	{
		public Hips hips;
		public Motor[] motors;

		public void SetParent (string newParent)
		{
			switch (newParent) 
			{
			case "hips" :
				
				Debug.Log( "hips" );
				MakeHipsTheParent();
				return;

			case "motor1" :
				
				Debug.Log( "motor 1" );
				MakeMotorTheParent( 0 );
				return;

			case "motor2" :
				
				Debug.Log( "motor 2" );
				MakeMotorTheParent( 1 );
				return;
			}
		}

		void MakeHipsTheParent ()
		{
			hips.transform.SetParent( transform );
			motors[0].transform.SetParent( hips.transform );
			motors[1].transform.SetParent( hips.transform );
		}

		void MakeMotorTheParent (int index)
		{
			motors[index].transform.SetParent( transform );
			hips.transform.SetParent( motors[index].transform );
			motors[index == 0 ? 1 : 0].transform.SetParent( hips.transform );
		}
	}
}