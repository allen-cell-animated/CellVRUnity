using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	[RequireComponent( typeof(AssemblyMolecule) )]
	public abstract class AssemblyMolecularComponent : MolecularComponent 
	{
		public AssemblyMolecule assemblyMolecule
		{
			get
			{
				return molecule as AssemblyMolecule;
			}
		}
	}
}