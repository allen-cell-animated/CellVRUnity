using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	[System.Serializable]
	public class BindingCriteria 
	{
		public MoleculeType typeToBind;
		public int siteId = 0;

		public BindingCriteria (MoleculeType _typeToBind, int _siteId)
		{
			typeToBind = _typeToBind;
			siteId = _siteId;
		}

		public virtual bool MatchesOther (BindingCriteria other)
		{
			return (other.typeToBind == typeToBind || typeToBind == MoleculeType.All || other.typeToBind == MoleculeType.All) 
				&& other.siteId == siteId;
		}
	}
}