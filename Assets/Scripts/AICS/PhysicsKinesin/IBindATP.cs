using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.PhysicsKinesin
{
	public interface IBindATP 
	{
		void CollideWithATP();

		void BindATP();

		void HydrolyzeATP();
	}
}