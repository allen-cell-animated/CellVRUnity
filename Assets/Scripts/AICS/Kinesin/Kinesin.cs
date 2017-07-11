using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Kinesin : MonoBehaviour 
	{
		//referenced parameters
		public float ejectionProbabilityMax = 0.9f;
		public float ejectionProbabilityMin = 0.01f;
		public float ejectionK = 30f;
		public float ejectionX0 = 0.65f;
		public bool useNecklinkerLogicForMotorEject = true;
		public float ATPBindProbabilityMax = 0.9f;
		public float ATPBindProbabilityMin = 0.01f;
		public float ATPBindK = 30f;
		public float ATPBindX0 = 0.75f;
		public float ADPReleaseProbabilityMax = 0.9f;
		public float ADPReleaseProbabilityMin = 0.01f;
		public float ADPReleaseK = 30f;
		public float ADPReleaseX0 = 0.75f;
		public float motorBindingRotationTolerance = 180f;
		public bool pushOtherMotorForwardAfterSnap = true;

		float maxDepenetrationVelocity = 20f;

		Hips _hips;
		public Hips hips
		{
			get {
				if (_hips == null)
				{
					_hips = GetComponentInChildren<Hips>();
				}
				return _hips;
			}
		}

		List<Motor> _motors;
		public List<Motor> motors
		{
			get {
				if (_motors == null)
				{
					_motors = new List<Motor>( GetComponentsInChildren<Motor>() );
				}
				return _motors;
			}
		}

		public Motor OtherMotor (Motor motor)
		{
			return motors.Find( m => m != motor );
		}

		void Start ()
		{
			SetParameters();
			AttachHipsToMotors();
			SetRigidbodyDepenetrationVelocity();
		}

		void SetParameters ()
		{
			//TODO (for parameter search)
		}

		void AttachHipsToMotors ()
		{
			Rigidbody[] lastLinks = new Rigidbody[motors.Count];
			for (int i = 0; i < motors.Count; i++)
			{
				lastLinks[i] = motors[i].neckLinker.lastLink.GetComponent<Rigidbody>();
			}
			hips.AttachToMotors( lastLinks );
		}

		void SetRigidbodyDepenetrationVelocity ()
		{
			Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
			foreach (Rigidbody body in bodies)
			{
				body.maxDepenetrationVelocity = maxDepenetrationVelocity;
			}
		}

		void Update ()
		{
			ReleaseWhenStretched();
		}

		void ReleaseWhenStretched ()
		{
			if (motors[0].neckLinker.stretched || motors[1].neckLinker.stretched)
			{
				if (motors[0].state == MotorState.Strong && motors[1].state == MotorState.Strong)
				{
					foreach (Motor motor in motors)
					{
						if (motor.neckLinker.snapping || motor.neckLinker.bound)
						{
							if (motor.printEvents) { Debug.Log( "kinesin released necklinker " + motor.name ); }
							motor.neckLinker.Release();
						}
					}
				}
				else 
				{
					foreach (Motor motor in motors)
					{
						if (motor.state == MotorState.Strong && motor.otherMotor.bound)
						{
							if (motor.inFront)
							{
								if (motor.otherMotor.printEvents) { Debug.Log( "kinesin released " + motor.otherMotor.name ); }
								motor.otherMotor.Eject();
							}
							else if (motor.neckLinker.snapping || motor.neckLinker.bound)
							{
								if (motor.printEvents) { Debug.Log( "kinesin released necklinker " + motor.name ); }
								motor.neckLinker.Release();
							}

						}
					}
				}
			}
		}

		public void SetHipsRotationLimits (Vector3 limits)
		{
			hips.SetJointRotationLimits( limits );
		}

		public void SetLinkRotationLimits (Vector3 limits)
		{
			foreach (Motor motor in motors)
			{
				motor.neckLinker.SetJointRotationLimits( limits );
			}
		}

		public void SetHipsMass (float mass)
		{
			hips.SetMass( mass );
		}

		public void SetMotorMass (float mass)
		{
			foreach (Motor motor in motors)
			{
				motor.SetMass( mass );
			}
		}

		public void SetLinkMass (float mass)
		{
			foreach (Motor motor in motors)
			{
				motor.neckLinker.SetLinkMass( mass );
			}
		}
	}
}
