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

		protected override MoleculeBinder PickFromValidBinders ()
		{
			if (centerMoleculeIsBound)
			{
				return GetFrontBinder();
			}
			else
			{
				return GetRandomBinder();
			}
		}
	}
}
