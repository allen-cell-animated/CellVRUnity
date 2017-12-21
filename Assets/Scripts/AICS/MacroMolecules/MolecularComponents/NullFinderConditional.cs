using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class NullFinderConditional : FinderConditional 
	{
		public override MoleculeBinder Find ()
		{
			return (GameObject.Instantiate( Resources.Load( "Tests/NullMolecule" ) as GameObject ) as GameObject).AddComponent<MoleculeBinder>();
		}
	}
}