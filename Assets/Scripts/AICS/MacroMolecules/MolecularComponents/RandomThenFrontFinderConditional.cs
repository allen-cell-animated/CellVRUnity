using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class RandomThenFrontFinderConditional : FrontFinderConditional 
	{
		bool centerMoleculeIsBound
		{
			get
			{
				return centerMolecule != null && centerMolecule.bound;
			}
		}

		protected override BindingSite PickFromValidBindingSites ()
		{
			if (centerMoleculeIsBound)
			{
				return GetFrontBindingSite();
			}
			else
			{
				return GetRandomBindingSite();
			}
		}
	}
}
