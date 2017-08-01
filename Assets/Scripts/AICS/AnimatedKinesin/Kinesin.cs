using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AnimatedKinesin
{
	public class Kinesin : MonoBehaviour 
	{
		public Hips hips;
		public List<Motor> motors;

		public void Release (string whichMotors)
		{
			switch (whichMotors) 
			{
			case "both" :

				ReleaseMotor( motors[0] );
				ReleaseMotor( motors[1] );
				return;

			case "motor1" :

				ReleaseMotor( motors[0] );
				return;

			case "motor2" :

				ReleaseMotor( motors[1] );
				return;
			}
		}

		public void MakeHipsTheParent ()
		{
			hips.transform.SetParent( null );
			motors[0].transform.SetParent( hips.transform );
			motors[1].transform.SetParent( hips.transform );
		}

		public void BindMotor (Motor motor)
		{
			Motor otherMotor = motors.Find( m => m != motor );
			if (otherMotor.state != MotorState.Free)
			{
				motor.transform.SetParent( null );
				hips.SetSecondParent( motor.transform );
			}
			else
			{
				motor.transform.SetParent( null );
				hips.transform.SetParent( motor.transform );
				otherMotor.transform.SetParent( hips.transform );
			}
		}

		public void ReleaseMotor (Motor motor)
		{
			Motor otherMotor = motors.Find( m => m != motor );
			if (otherMotor.state != MotorState.Free)
			{
				hips.transform.SetParent( otherMotor.transform );
				motor.transform.SetParent( hips.transform );
				hips.SetSecondParent( null );
			}
			else
			{
				hips.transform.SetParent( null );
				motors[0].transform.SetParent( hips.transform );
				motors[1].transform.SetParent( hips.transform );
			}
			motor.Release();
		}
	}
}