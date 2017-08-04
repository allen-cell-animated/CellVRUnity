using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AnimatedKinesin
{
	public class Kinesin : MonoBehaviour 
	{
		public bool logEvents;
		public int maxIterations = 50;
		public List<Molecule> molecules;
		public TubulinDetector tubulinDetector;
		public float ATPBindingProbabilityFront = 70f;
		public float ATPBindingProbabilityBack = 10f;
		public float ejectionProbabilityFront = 70f;
		public float ejectionProbabilityBack = 30f;
		public float averageWalkingSpeed;

		Vector3 hipsStartPosition;
		Vector3 motor1StartPosition;
		Vector3 motor2StartPosition;
		string lastState = "";
		float startTime = 0;

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

		void Awake ()
		{
			Application.targetFrameRate = 30;

			hipsStartPosition = hips.transform.position;
			motor1StartPosition = motors[0].transform.position;
			motor2StartPosition = motors[1].transform.position;

			foreach (Molecule molecule in molecules)
			{
				molecule.kinesin = this;
			}
		}

		void LateUpdate ()
		{
			Simulate();

			CalculateWalkingSpeed();
		}

		void Simulate ()
		{
			DoRandomWalks();

			string state = "";
			if (MotorStatesAre( MotorState.Free, MotorState.Free )) // case 200
			{
				state = "200";
				// check for free motor binding
			}
			else if (MotorStatesAre( MotorState.Free, MotorState.Weak )) // case 211
			{
				state = "211";
				// check for free motor binding
				CheckBindATP( MotorInState( MotorState.Weak ), ATPBindingProbabilityBack ); // should be 10%
				// check for weak motor eject? (in state machine but not C4D)
			}
			else if (MotorStatesAre( MotorState.Free, MotorState.Strong ) && hips.state == HipsState.Free) // case 312
			{
				state = "312";
				// check for free motor binding
				if (lastState != state)
				{
					hips.StartSnap( MotorInState( MotorState.Strong ) );
				}
				hips.UpdateSnap();
				// check for strong motor eject? (in state machine but not C4D)
			}
			else if (MotorStatesAre( MotorState.Free, MotorState.Strong ) && hips.state == HipsState.Locked) // case 333
			{
				state = "333";
				// check for free motor binding
				// check for strong motor eject? (in state machine but not C4D)
			}
			else if (MotorStatesAre( MotorState.Weak, MotorState.Strong )) // case 345  && hips.state == HipsState.Locked
			{
				state = "345";
				Motor initialStrongMotor = MotorInState( MotorState.Strong );
				if (CheckBindATP( MotorInState( MotorState.Weak ), ATPBindingProbabilityFront ))
				{
					CheckEject( initialStrongMotor, ejectionProbabilityFront );
				}
				else
				{
					// check for weak motor eject? (in state machine but not C4D)
					CheckEject( initialStrongMotor, ejectionProbabilityBack );
				}
			}
			else if (MotorStatesAre( MotorState.Strong, MotorState.Strong )) // case 447
			{
				state = "447";
				Motor motor = motors.Find( m => m.hipsAreLockedToThis );
				CheckEject( motor != null ? motor : backMotor, ejectionProbabilityFront );
			}
			else if (MotorStatesAre( MotorState.Weak, MotorState.Weak )) // case 7: 212?
			{
				state = "case 7"; // in state machine but not C4D, sim gets stuck without it
				CheckBindATP( frontMotor, ATPBindingProbabilityFront ); // is this right and what are actual probabilities?
				CheckBindATP( backMotor, ATPBindingProbabilityBack );
				// check for weak motor eject? (in state machine but not C4D)
			}
			else 
			{
				state = "none";
			}

			if (state != lastState )
			{
				if (logEvents) { Debug.Log( state ); }
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
				motor.transform.SetParent( transform );
				hips.SetSecondParent( motor.transform );
			}
			else
			{
				motor.transform.SetParent( transform );
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
				hips.SetSecondParent( transform );
			}
			else
			{
				SetHipsAsParent();
			}
		}

		void SetHipsAsParent ()
		{
			hips.transform.SetParent( transform );
			motors[0].transform.SetParent( hips.transform );
			motors[1].transform.SetParent( hips.transform );
		}

		public void Reset ()
		{
			foreach (Molecule molecule in molecules)
			{
				molecule.Reset();
			}
			SetHipsAsParent();

			hips.transform.position = hipsStartPosition;
			motors[0].transform.position = motor1StartPosition;
			motors[1].transform.position = motor2StartPosition;
			startTime = Time.time;
		}

		public void SetNeckLinkerMinLength (float min)
		{
			foreach (Molecule molecule in molecules)
			{
				molecule.minDistanceFromParent = min;
			}
		}

		public void SetNeckLinkerMaxLength (float max)
		{
			foreach (Molecule molecule in molecules)
			{
				molecule.maxDistanceFromParent = max;
			}
		}

		public void SetMeanStepSize (float size)
		{
			foreach (Molecule molecule in molecules)
			{
				molecule.meanStepSize = size;
			}
		}

		void CalculateWalkingSpeed ()
		{
			averageWalkingSpeed = (hips.transform.position - hipsStartPosition).magnitude / (Time.time - startTime);
		}
	}
}