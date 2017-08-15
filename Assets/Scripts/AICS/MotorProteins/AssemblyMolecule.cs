using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins
{
	// An assembly that is made up of molecules
	public abstract class AssemblyMolecule : Molecule 
	{
		public List<ComponentMolecule> molecules;
		public KineticRates kineticRates;

		public bool CheckCollision (Molecule molecule, Vector3 moveStep)
		{
			foreach (ComponentMolecule m in molecules)
			{
				if (m as Molecule != molecule)
				{
					if (Vector3.Distance( m.transform.position, molecule.transform.position + moveStep ) <= m.radius + molecule.radius)
					{
						return true;
					}
				}
			}
			return false;
		}

		public abstract void SetParentSchemeOnComponentBind (ComponentMolecule molecule);

		public abstract void SetParentSchemeOnComponentRelease (ComponentMolecule molecule);

		public void SetMinDistanceFromParent (float min)
		{
			foreach (ComponentMolecule molecule in molecules)
			{
				molecule.minDistanceFromParent = min;
			}
		}

		public void SetMaxDistanceFromParent (float max)
		{
			foreach (ComponentMolecule molecule in molecules)
			{
				molecule.maxDistanceFromParent = max;
			}
		}

		public void SetMeanStepSize (float size)
		{
			foreach (ComponentMolecule molecule in molecules)
			{
				molecule.meanStepSize = size;
			}
		}
	}
}