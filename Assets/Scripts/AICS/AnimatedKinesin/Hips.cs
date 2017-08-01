using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AnimatedKinesin
{
	public class Hips : Molecule 
	{
		Transform secondParent = null;

		void LateUpdate () 
		{
			Move();
//			Rotate();
		}

		protected override bool WillCollide (Vector3 moveStep)
		{
			RaycastHit hit;
			if (body.SweepTest( moveStep.normalized, out hit, moveStep.magnitude, UnityEngine.QueryTriggerInteraction.Collide ))
			{
				return true;
			}
			return false;
		}

		public void SetSecondParent (Transform _secondParent)
		{
			secondParent = _secondParent;
		}

		protected override bool WithinLeash (Vector3 moveStep)
		{
			return (secondParent == null || Vector3.Distance( secondParent.position, transform.position + moveStep ) <= maxDistanceFromParent)
				&& Vector3.Distance( transform.parent.position, transform.position + moveStep ) <= maxDistanceFromParent;
		}

		protected override Vector3 towardLeashDirection
		{
			get
			{
				return ((secondParent != null ? (transform.parent.position + secondParent.position) / 2f : transform.parent.position) - transform.position).normalized;
			}
		}
	}
}