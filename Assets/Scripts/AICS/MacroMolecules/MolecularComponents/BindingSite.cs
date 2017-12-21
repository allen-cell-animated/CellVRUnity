using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class BindingSite : MolecularComponent
	{
		public BindingCriteria criteria;
		public MoleculeBinder boundBinder;

		public bool IsAvailableMatch (BindingCriteria _criteria)
		{
			return boundBinder == null && criteria.MatchesOther( _criteria );
		}
	}
}