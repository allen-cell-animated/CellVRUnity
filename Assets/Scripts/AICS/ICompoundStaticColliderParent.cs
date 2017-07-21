using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public interface ICompoundStaticColliderParent 
	{
		void Collided (Collision collision);
	}
}