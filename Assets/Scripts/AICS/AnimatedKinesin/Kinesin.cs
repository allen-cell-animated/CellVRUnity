using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AnimatedKinesin
{
	public class Kinesin : MonoBehaviour 
	{
		public int maxIterations = 50;
		public List<Molecule> molecules;
		public TubulinDetector tubulinDetector;

		Hips _hips;
		public Hips hips
		{
			get
			{
				if (_hips == null)
				{
					_hips = molecules.Find( m => m.GetComponent<Hips>() != null ) as Hips;
				}
				return _hips;
			}
		}

		List<Motor> _motors;
		public List<Motor> motors
		{
			get
			{
				if (_motors == null)
				{
					_motors = new List<Motor>();
					foreach (Molecule molecule in molecules)
					{
						Motor motor = molecule.GetComponent<Motor>();
						if (motor != null)
						{
							_motors.Add( motor );
						}
					}
				}
				return _motors;
			}
		}

		void LateUpdate ()
		{
			Simulate();
		}

		void Simulate ()
		{
			DoRandomWalks();

			foreach (Motor motor in motors)
			{
				CheckEject( motor, 10f );
			}
//			if (motors[0].state == MotorState.Free && motors[1].state == MotorState.Free) // case 200
//			{
//
//			}
//			else if (motors[0].state == MotorState.Free && motors[1].state != MotorState.Free
//				|| motors[0].state != MotorState.Free && motors[1].state == MotorState.Free)
//			{
//
//			}
		}

		void DoRandomWalks ()
		{
			foreach (Molecule molecule in molecules)
			{
				molecule.DoRandomWalk();
			}
		}

		void CheckBindATP (Motor motor, float probability)
		{
			if (DiceRoll( probability ))
			{

			}
		}

		void CheckEject (Motor motor, float probability)
		{
			if (DiceRoll( probability ))
			{
				ReleaseMotor( motor );
			}
		}

		bool DiceRoll (float probability)
		{
			return Random.Range( 0, 1f ) <= Time.deltaTime * probability / 100f;
		}

		public bool CheckInternalCollision (Molecule molecule, Vector3 moveStep)
		{
			foreach (Molecule m in molecules)
			{
				if (m != molecule)
				{
					if (Vector3.Distance( m.transform.position, molecule.transform.position + moveStep ) <= m.radius + molecule.radius)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void SetParentSchemeOnBind (Motor motor)
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

		public void SetParentSchemeOnRelease (Motor motor)
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
		}

		// for testing ------------------------------------------

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

		void ReleaseMotor (Motor motor)
		{
			SetParentSchemeOnRelease( motor );
			motor.Release();
		}
	}
}