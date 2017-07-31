using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AnimatedKinesin
{
	public class Hips : Molecule 
	{
		void FixedUpdate () 
		{
			Rotate();
			Move();
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
	}
}