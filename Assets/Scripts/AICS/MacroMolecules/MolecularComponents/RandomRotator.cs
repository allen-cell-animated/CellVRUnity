using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class RandomRotator : MolecularComponent, ISimulate
	{
		public float meanRotation = 5f;

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
	}
}