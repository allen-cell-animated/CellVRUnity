using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class NullFinderConditional : FinderConditional 
	{
		public override BindingSite Find ()
		{
			BindingSite site = (GameObject.Instantiate( Resources.Load( "Tests/NullMolecule" ) as GameObject ) as GameObject).AddComponent<BindingSite>();
			site.criteria = bindingCriteria;
			return site;
		}
	}
}