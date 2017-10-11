using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class RandomMover : MolecularComponent, ISimulate
	{
		public float meanStepSize = 2f;

		public void DoSimulationStep ()
		{
			Move();
		}

		public bool Move () 
		{
			Vector3 moveStep = Helpers.GetRandomVector( Helpers.SampleExponentialDistribution( meanStepSize ) );
			return molecule.MoveIfValid( moveStep );
		}
	}
}