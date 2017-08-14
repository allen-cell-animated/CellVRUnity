using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.AnimatedKinesin
{
	// A molecule that is a commponent of a molecular assembly
	public abstract class ComponentMolecule : Molecule 
	{
		public AssemblyMolecule assembly;
		public float minDistanceFromParent = 2f;
		public float maxDistanceFromParent = 6f;

		bool isParent
		{
			get
			{
				return transform.parent == assembly.transform;
			}
		}

		protected override bool IsValidMove (Vector3 moveStep)
		{
			return isParent || CheckLeash( moveStep );
		}

		protected abstract bool CheckLeash (Vector3 moveStep);
	}
}