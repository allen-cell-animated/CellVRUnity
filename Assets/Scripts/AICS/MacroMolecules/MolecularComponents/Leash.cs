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

		public Molecule GetMonomerClosestTo (Molecule monomerToFind)
		{
			if (attachedMolecule == monomerToFind)
			{
				return attachedMolecule;
			}
			foreach (Leash leash in attachedMolecule.leashes)
			{
				if (leash.attachedMolecule != molecule)
				{
					if (leash.attachedMolecule == monomerToFind)
					{
						return leash.molecule;
					}
					Molecule m = leash.GetMonomerClosestTo( monomerToFind );
					if (m != null)
					{
						return m;
					}
				}
			}
			return null;
		}

		public int GetMinBranchesToMolecule (Molecule moleculeToFind)
		{
			if (molecule == moleculeToFind)
			{
				return 0;
			}
			if (attachedMolecule == moleculeToFind)
			{
				return 1;
			}

			int n, min = int.MaxValue - 10;
			foreach (Leash leash in attachedMolecule.leashes)
			{
				if (leash.attachedMolecule != molecule)
				{
					n = leash.GetMinBranchesToMolecule( moleculeToFind );
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
