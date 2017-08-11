using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AnimatedKinesin
{
	public class Kinesin : MonoBehaviour 
	{
		public List<Molecule> molecules;
		public KineticRates kineticRates;
		public float nanosecondsPerStep = 1E5f;
		public int stepsPerFrame = 1;
		public int maxIterationsPerStep = 50;
		public float averageWalkingSpeed; // μm/s

		Vector3 hipsStartPosition;
		Vector3 motor1StartPosition;
		Vector3 motor2StartPosition;
		float startTime = 0;
		float speedMultiplier;

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
			Application.targetFrameRate = -1;
			speedMultiplier = 1E-3f / (nanosecondsPerStep * 1E-9f * stepsPerFrame);

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
			for (int i = 0; i < stepsPerFrame; i++)
			{
				foreach (Molecule molecule in molecules)
				{
					molecule.Simulate();
				}
			}

			CalculateWalkingSpeed();
		}

		public void SetParentSchemeOnBind (Motor motor)
		{
			Motor otherMotor = motors.Find( m => m != motor );
			if (otherMotor.bound)
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
			if (otherMotor.bound)
			{
				hips.transform.SetParent( otherMotor.transform );
				motor.transform.SetParent( hips.transform );
			}
			else
			{
				SetHipsAsParent();
			}
			hips.SetSecondParent( null );
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
			averageWalkingSpeed = Time.deltaTime * speedMultiplier * (hips.transform.position - hipsStartPosition).magnitude / (Time.time - startTime);
		}
	}
}