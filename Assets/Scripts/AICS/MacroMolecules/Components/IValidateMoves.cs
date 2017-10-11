using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public interface IValidateMoves 
	{
		bool MoveIsValid (Vector3 position, float radius);
	}
}