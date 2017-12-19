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

		public Molecule GetComponentClosestTo (Molecule componentToFind)
		{
			if (attachedMolecule == componentToFind)
			{
				return attachedMolecule;
			}
			foreach (Leash leash in attachedMolecule.leashes)
			{
				if (leash.attachedMolecule != molecule)
				{
					if (leash.attachedMolecule == componentToFind)
					{
						return leash.molecule;
					}
					Molecule m = leash.GetComponentClosestTo( componentToFind );
					if (m != null)
					{
						return m;
					}
				}
			}
			return null;
		}

		public int GetMinBranchesToComponent (Molecule componentToFind)
		{
			if (molecule == componentToFind)
			{
				return 0;
			}
			if (attachedMolecule == componentToFind)
			{
				return 1;
			}

			int n, min = int.MaxValue - 10;
			foreach (Leash leash in attachedMolecule.leashes)
			{
				if (leash.attachedMolecule != molecule)
				{
					n = leash.GetMinBranchesToComponent( componentToFind );
					if (n < min)
					{
						min = n;
					}
				}
			}
			return min + 1;
		}
	}
}
