using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MacroMolecules
{
	public class OrientedLeash : Leash 
	{
		public Transform orientor = null;
		public Direction axisToOrientTo = Direction.up;
		public float maxAngleFromUp = 30f;

		protected override bool CheckLeash (Vector3 moveStep)
		{
			if (base.CheckLeash( moveStep ))
			{
				if (orientor == null)
				{
					return true;
				}
				return AngleFromCenter( moveStep ) <= maxAngleFromUp;
			}
			return false;
		}

		float AngleFromCenter (Vector3 moveStep)
		{
			Vector3 axis = Helpers.GetLocalDirection( axisToOrientTo, orientor );
			Vector3 parentToNewPosition = (transform.position + moveStep - transform.parent.position).normalized;
			return Mathf.Acos( Vector3.Dot( axis, parentToNewPosition ) ) * Mathf.Rad2Deg;
		}
	}
}
