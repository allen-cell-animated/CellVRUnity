using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	}
}