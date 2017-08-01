using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AnimatedKinesin
{
	public class Hips : Molecule 
	{
		Transform secondParent = null;

		void FixedUpdate () 
		{
			Rotate();
			Move();
		}

		protected override bool WillCollide (Vector3 moveStep)
		{
			RaycastHit hit;
			if (body.SweepTest( moveStep.normalized, out hit, 2f * moveStep.magnitude, UnityEngine.QueryTriggerInteraction.Collide ))
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
				return ((transform.parent.position + (secondParent != null ? secondParent.position : Vector3.zero)) / 2f - transform.position).normalized;
			}
		}

		// for testing
		public float distanceFromParent;
		public float distanceFromSecondParent;

		void Update ()
		{
			if (transform.parent != null)
			{
				distanceFromParent = Vector3.Distance( transform.position, transform.parent.position );
			}
			if (secondParent != null)
			{
				distanceFromSecondParent = Vector3.Distance( transform.position, secondParent.position );
			}
		}
	}
}