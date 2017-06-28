using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Kinesin : MonoBehaviour 
	{
		public float motorReleaseProbabilityMax = 0.9f;
		public float motorReleaseProbabilityMin = 0.01f;
		public float ATPBindProbabilityMax = 0.9f;
		public float ATPBindProbabilityMin = 0.01f;
		public float ADPReleaseProbabilityMax = 0.9f;
		public float ADPReleaseProbabilityMin = 0.01f;

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
			AttachHipsToMotors();
		}

		void AttachHipsToMotors ()
		{
			Rigidbody[] lastLinks = new Rigidbody[motors.Count];
			for (int i = 0; i < motors.Count; i++)
			{
				lastLinks[i] = motors[i].neckLinker.lastLink;
			}
			hips.AttachToMotors( lastLinks );
		}

		void Update ()
		{
			if (motors[0].neckLinker.stretched || motors[1].neckLinker.stretched)
			{
				if (motors[0].state == MotorState.Strong && motors[1].state == MotorState.Strong)
				{
					if (!motors[0].neckLinker.tensionIsForward)
					{
						Debug.Log( "kinesin release motor1 necklinker" );
						motors[0].neckLinker.Release();
					}
					else if (!motors[1].neckLinker.tensionIsForward)
					{
						Debug.Log( "kinesin release motor2 necklinker" );
						motors[1].neckLinker.Release();
					}
				}
				else if (motors[0].state == MotorState.Strong)
				{
					Debug.Log( "kinesin release motor2" );
					motors[1].Release();
				}
				else if (motors[1].state == MotorState.Strong)
				{
					Debug.Log( "kinesin release motor1" );
					motors[0].Release();
				}
			}
		}
	}
}
