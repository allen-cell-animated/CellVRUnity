using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public interface IHandleBinds 
	{
		void SubscribeToBindEvents (List<MoleculeBinder> binders);
	}
}