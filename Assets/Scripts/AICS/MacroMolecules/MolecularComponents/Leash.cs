using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class Leash : MolecularComponent, IValidateMoves
	{
		public Molecule attachedMolecule;
		public bool dynamicLength = true;
		public float minDistanceFromParent;
		public float maxDistanceFromParent;

		public bool MoveIsValid (Vector3 position, float radius)
		{
			return CheckLeash( position );
		}

		protected virtual bool CheckLeash (Vector3 position)
		{
			float d = Vector3.Distance( attachedMolecule.transform.position, position );
			return d >= minDistanceFromParent && d <= maxDistanceFromParent;
		}

		public void SetMinDistanceFromParent (float min)
		{
			if (dynamicLength)
			{
				minDistanceFromParent = min;
			}
		}

		public void SetMaxDistanceFromParent (float max)
		{
			if (dynamicLength)
			{
				maxDistanceFromParent = max;
			}
		}
	}
}
