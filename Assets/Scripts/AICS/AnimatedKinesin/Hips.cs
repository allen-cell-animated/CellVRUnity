using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.AnimatedKinesin
{
	public enum HipsState 
	{
		Free,
		Locked
	}

	public class Hips : Molecule 
	{
		public HipsState state = HipsState.Free;
		public float snapPosition = 4.5f; // nm in front of motor pivot

		Transform secondParent = null;

		protected override bool canMove
		{
			get
			{
				return state != HipsState.Locked;
			}
		}

		public override void DoRandomWalk ()
		{
			if (canMove)
			{
				Rotate();
				for (int i = 0; i < kinesin.maxIterations; i++)
				{
					if (Move())
					{
						return;
					}
				}
				Jitter( 0.1f );
			}
		}

		public bool CheckForSnap (Motor strongMotor)
		{
			Vector3 bindingPosition = SnappedPosition( strongMotor );
			if (Vector3.Distance( transform.position, bindingPosition ) <= 4f)
			{
				SnapDown( bindingPosition );
				strongMotor.BindNecklinker();
				return true;
			}
			return false;
		}

		Vector3 SnappedPosition (Motor strongMotor)
		{
			return strongMotor.transform.position + snapPosition * strongMotor.transform.forward;
		}

		void SnapDown (Vector3 bindingPosition)
		{
			transform.position = bindingPosition;
			state = HipsState.Locked;
		}

		public void SetFree ()
		{
			state = HipsState.Free;
		}

		protected override void OnCollisionWithTubulin (Tubulin[] collidingTubulins) { }

		public void SetSecondParent (Transform _secondParent)
		{
			secondParent = _secondParent;
		}

		protected override bool WithinLeash (Vector3 moveStep)
		{
			if (secondParent != null)
			{
				float d2 = Vector3.Distance( secondParent.position, transform.position + moveStep );
				if (d2 < minDistanceFromParent || d2 > maxDistanceFromParent)
				{
					return false;
				}
			}
			float d1 = Vector3.Distance( transform.parent.position, transform.position + moveStep );
			return d1 >= minDistanceFromParent && d1 <= maxDistanceFromParent;
		}

		public override void Reset ()
		{
			state = HipsState.Free;
			secondParent = null;
		}
	}
}