using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public interface ICompoundStaticColliderParent 
	{
		void Collided (Collision collision);
	}
}