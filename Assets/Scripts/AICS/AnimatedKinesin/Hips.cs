using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.AnimatedKinesin
{
	public class Hips : Molecule 
	{
		Transform secondParent = null;

		protected override bool canMove
		{
			get
			{
				return true;
			}
		}

		public override void DoRandomWalk ()
		{
			Rotate();
			for (int i = 0; i < kinesin.maxIterations; i++)
			{
				if (Move())
				{
					return;
				}
			}
		}

		protected override void OnCollisionWithTubulin (Tubulin[] collidingTubulins) { }

		public void SetSecondParent (Transform _secondParent)
		{
			secondParent = _secondParent;
		}

		protected override bool WithinLeash (Vector3 moveStep)
		{
			return (secondParent == null || Vector3.Distance( secondParent.position, transform.position + moveStep ) <= maxDistanceFromParent)
				&& Vector3.Distance( transform.parent.position, transform.position + moveStep ) <= maxDistanceFromParent;
		}
	}
}