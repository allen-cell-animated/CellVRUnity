using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AnimatedKinesin
{
	public class Kinesin : MonoBehaviour 
	{
		public Hips hips;
		public List<Motor> motors;

		// For testing with UI
		public void SetParent (string newParent)
		{
			switch (newParent) 
			{
			case "hips" :
				
				MakeHipsTheParent();
				return;

			case "motor1" :
				
				MakeMotorTheParent( motors[0] );
				return;

			case "motor2" :
				
				MakeMotorTheParent( motors[1] );
				return;
			}
		}

		public void MakeHipsTheParent ()
		{
			hips.transform.SetParent( transform );
			motors[0].transform.SetParent( hips.transform );
			motors[1].transform.SetParent( hips.transform );
		}

		public void MakeMotorTheParent (Motor parentMotor)
		{
			parentMotor.transform.SetParent( transform );
			hips.transform.SetParent( parentMotor.transform );
			Motor otherMotor = motors.Find( m => m != parentMotor );
			otherMotor.transform.SetParent( hips.transform );
			otherMotor.Release();
		}
	}
}