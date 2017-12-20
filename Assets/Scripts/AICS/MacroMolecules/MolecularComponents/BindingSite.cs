using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class BindingSite : MolecularComponent
	{
		public MoleculeType typeToBind;
		public MoleculeBinder boundBinder;
	}
}