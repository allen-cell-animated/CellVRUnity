using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class RandomRotator : MolecularComponent, ISimulate
	{
		public float meanRotation = 5f;
		public bool dynamicMeanRotation = true;

		public void DoSimulationStep ()
		{
			Rotate();
		}

		public void Rotate ()
		{
			if (molecule.canMove)
			{
				transform.rotation *= Quaternion.Euler( Helpers.GetRandomVector( Helpers.SampleExponentialDistribution( meanRotation ) ) );
			}
		}

		public void SetMeanRotation (float _meanRotation)
		{
			if (dynamicMeanRotation)
			{
				meanRotation = _meanRotation;
			}
		}
	}
}