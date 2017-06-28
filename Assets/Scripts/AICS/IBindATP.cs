using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public interface IBindATP 
	{
		void CollideWithATP();

		void BindATP();

		void HydrolyzeATP();
	}
}