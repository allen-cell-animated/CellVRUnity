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

		string lastState = "";

		void Simulate ()
		{
			DoRandomWalks();

			string state = "";
			if (MotorStatesAre( MotorState.Free, MotorState.Free )) // case 200
			{
				state = "200";
				// check for binding when a motor collides with a tubulin as part of random walk
			}
			else if (MotorStatesAre( MotorState.Free, MotorState.Weak )) // case 211
			{
				state = "211";
				// check for free motor binding when it collides with a tubulin as part of random walk
				CheckBindATP( MotorInState( MotorState.Weak ), 10f );
				// check for weak motor eject? (in state machine but not C4D)
			}
			else if (MotorStatesAre( MotorState.Free, MotorState.Strong ) && hips.state == HipsState.Free) // case 312
			{
				state = "312";
				hips.CheckForSnap( MotorInState( MotorState.Strong ) );
				// check for strong motor eject? (in state machine but not C4D)
				// check for free motor binding when it collides with a tubulin as part of random walk (in state machine but not C4D)
			}
			else if (MotorStatesAre( MotorState.Free, MotorState.Strong ) && hips.state == HipsState.Locked) // case 333
			{
				state = "333";
				// check for free motor binding when it collides with a tubulin as part of random walk
				// check for strong motor eject? (in state machine but not C4D)
			}
			else if (MotorStatesAre( MotorState.Weak, MotorState.Strong )) // case 345  && hips.state == HipsState.Locked
			{
				state = "345";
				Motor initialStrongMotor = MotorInState( MotorState.Strong );
				if (CheckBindATP( MotorInState( MotorState.Weak ), 70f ))
				{
					CheckEject( initialStrongMotor, 70f );
				}
				else
				{
					// check for weak motor eject? (in state machine but not C4D)
					CheckEject( initialStrongMotor, 30f );
				}
			}
			else if (MotorStatesAre( MotorState.Strong, MotorState.Strong )) // case 447
			{
				state = "447";
				Motor motor = motors.Find( m => m.hipsAreLockedToThis );
				CheckEject( motor != null ? motor : backMotor, 70f );
			}
			else if (MotorStatesAre( MotorState.Weak, MotorState.Weak )) // case 7: 212?
			{
				state = "case 7"; // in state machine but not C4D, sim gets stuck without it
//				CheckBindATP( frontMotor, 80f ); // is this right and what are actual probabilities?
//				CheckBindATP( backMotor, 10f );
				CheckEject( backMotor, 70f );
			}
			else 
			{
				state = "none";
			}

			if (state != lastState)
			{
				Debug.Log( state );
				lastState = state;
			}
		}

		bool MotorStatesAre (MotorState state1, MotorState state2)
		{
			if (state1 == state2)
			{
				return motors[0].state == state1 && motors[1].state == state1;
			}
			return (motors[0].state == state1 && motors[1].state == state2)
				|| (motors[0].state == state2 && motors[1].state == state1);
		}

		Motor MotorInState (MotorState state)
		{
			return motors.Find( m => m.state == state );
		}

		Motor frontMotor
		{
			get
			{
				Vector3 motor1ToHips = (hips.transform.position - motors[0].transform.position).normalized;
				float angle = Mathf.Acos( Vector3.Dot( motor1ToHips, motors[0].transform.forward ) );
				if (angle < Mathf.PI / 2f)
				{
					return motors[1];
				}
				else
				{
					return motors[0];
				}
			}
		}

		Motor backMotor
		{
			get
			{
				return motors.Find( m => m != frontMotor );
			}
		}

		void DoRandomWalks ()
		{
			foreach (Molecule molecule in molecules)
			{
				molecule.DoRandomWalk();
			}
		}

		bool CheckBindATP (Motor motor, float probability)
		{
			if (DiceRoll( probability ))
			{
				motor.BindATP();
				return true;
			}
			return false;
		}

		bool CheckEject (Motor motor, float probability)
		{
			if (DiceRoll( probability ))
			{
				SetParentSchemeOnRelease( motor );
				motor.Release();
				return true;
			}
			return false;
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