using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class RandomMover : MolecularComponent, ISimulate
	{
		public float meanStepSize = 2f;
		public bool dynamicMeanStepSize = true;

		public void DoSimulationStep ()
		{
			for (int i = 0; i < MolecularEnvironment.Instance.maxIterationsPerStep; i++)
			{
				if (Move())
				{
					return;
				}
			}
		}

		public bool Move () 
		{
			Vector3 moveStep = Helpers.GetRandomVector( Helpers.SampleExponentialDistribution( meanStepSize ) );
			return molecule.MoveIfValid( moveStep );
		}

		public void SetMeanStepSize (float _stepSize)
		{
			if (dynamicMeanStepSize)
			{
				meanStepSize = _stepSize;
			}
		}
	}
}