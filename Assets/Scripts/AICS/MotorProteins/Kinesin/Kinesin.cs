using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.MotorProteins.Kinesin
{
	public class Kinesin : AssemblyMolecule 
	{
		public float averageWalkingSpeed; // μm/s
		public Tubulin lastTubulin;

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

		void Update ()
		{
			for (int i = 0; i < MolecularEnvironment.Instance.stepsPerFrame; i++)
			{
				Simulate();
			}
			CalculateWalkingSpeed();
		}

		public override void DoCustomSimulation ()
		{
			foreach (ComponentMolecule molecule in componentMolecules)
			{
				molecule.Simulate();
			}
		}

		public override void DoCustomReset ()
		{
			foreach (Molecule molecule in componentMolecules)
			{
				molecule.Reset();
			}
			SetHipsAsParent();
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
			averageWalkingSpeed = 1E-3f * (hips.transform.position - hips.startPosition).magnitude / (nanosecondsSinceStart * 1E-9f);
		}
	}
}