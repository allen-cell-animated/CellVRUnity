using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class NullFinderConditional : FinderConditional 
	{
		public override MoleculeBinder Find ()
		{
			GameObject molecule = GameObject.Instantiate( Resources.Load( "Tests/NullMolecule" ) as GameObject ) as GameObject;
			return molecule.AddComponent<MoleculeBinder>();
		}
	}
}