using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.MotorProteins
{
	// A component molecule that links two others
	public abstract class LinkerComponentMolecule : ComponentMolecule 
	{
		public Transform secondParent = null;

		protected override bool CheckLeash (Vector3 moveStep)
		{
			if (secondParent != null)
			{
				float d2 = Vector3.Distance( secondParent.position, transform.position + moveStep );
				if (d2 < minDistanceFromParent || d2 > maxDistanceFromParent)
				{
					return false;
				}
			}
			float d1 = Vector3.Distance( transform.parent.position, transform.position + moveStep );
			return d1 >= minDistanceFromParent && d1 <= maxDistanceFromParent;
		}

		public void SetSecondParent (Transform _secondParent)
		{
			secondParent = _secondParent;
		}

		protected void RelaxBetweenParents ()
		{
			if (transform.parent != null && secondParent != null)
			{
				transform.position = (transform.parent.position + secondParent.position) / 2f;
//				Vector3 midpoint = (transform.parent.position + secondParent.position) / 2f;
//				Vector3 moveStep = SampleExponentialDistribution( meanStepSize ) * (midpoint - transform.position).normalized;
//				if (IsValidMove( Vector3.zero ))
//				{
//					MoveIfValid( moveStep );
//				}
//				else
//				{
//					transform.position += moveStep;
//				}
			}
		}
	}
}