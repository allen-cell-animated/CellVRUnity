﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins.Kinesin
{
	public class Kinesin : AssemblyMolecule 
	{
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
					_hips = componentMolecules.Find( m => m.GetComponent<Hips>() != null ) as Hips;
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
					foreach (ComponentMolecule molecule in componentMolecules)
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

		public override bool bound
		{
			get
			{
				foreach (Motor motor in motors)
				{
					if (motor.bound)
					{
						return true;
					}
				}
				return false;
			}
		}

		void Awake ()
		{
			ConnectComponents();

			speedMultiplier = 1E-3f / (MolecularEnvironment.Instance.nanosecondsPerStep * 1E-9f * MolecularEnvironment.Instance.stepsPerFrame);

			hipsStartPosition = hips.transform.position;
			motor1StartPosition = motors[0].transform.position;
			motor2StartPosition = motors[1].transform.position;
		}

		void Update ()
		{
			for (int i = 0; i < MolecularEnvironment.Instance.stepsPerFrame; i++)
			{
				Simulate();
			}
			CalculateWalkingSpeed();
		}

		public override void Simulate ()
		{
			foreach (ComponentMolecule molecule in componentMolecules)
			{
				molecule.Simulate();
			}
		}

		public override void Reset ()
		{
			foreach (Molecule molecule in componentMolecules)
			{
				molecule.Reset();
			}
			SetHipsAsParent();

			hips.transform.position = hipsStartPosition;
			motors[0].transform.position = motor1StartPosition;
			motors[1].transform.position = motor2StartPosition;
			startTime = Time.time;
		}

		public override void SetParentSchemeOnComponentBind (ComponentMolecule molecule)
		{
			Motor motor = molecule as Motor;
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
			ResetComponentScale();
		}

		public override void SetParentSchemeOnComponentRelease (ComponentMolecule molecule)
		{
			Motor motor = molecule as Motor;
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
			ResetComponentScale();
		}

		void SetHipsAsParent ()
		{
			hips.transform.SetParent( transform );
			motors[0].transform.SetParent( hips.transform );
			motors[1].transform.SetParent( hips.transform );
		}

		void CalculateWalkingSpeed ()
		{
			averageWalkingSpeed = Time.deltaTime * speedMultiplier * (hips.transform.position - hipsStartPosition).magnitude / (Time.time - startTime);
		}
	}
}