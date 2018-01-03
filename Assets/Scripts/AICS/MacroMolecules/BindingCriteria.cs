using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	[System.Serializable]
	public class BindingCriteria 
	{
		public MoleculeType type;
		public int siteId = 0;

		public BindingCriteria (MoleculeType _typeToBind, int _siteId)
		{
			type = _typeToBind;
			siteId = _siteId;
		}

		public virtual bool MatchesOther (BindingCriteria other)
		{
			return (other.type == type || type == MoleculeType.All || other.type == MoleculeType.All) 
				&& other.siteId == siteId;
		}
	}
}