using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.AnimatedKinesin
{
	public enum MotorState 
	{
		Free,
		Weak,
		Strong
	}

	public class Motor : Molecule 
	{
		public MotorState state = MotorState.Free;
		public float bindingRotationTolerance = 30f;
		public bool hipsAreLockedToThis;
		public GameObject ATP;

		Vector3 bindingPosition = new Vector3( 0.34f, 4.01f, 0.34f );
		Vector3 bindingRotation = new Vector3( 357.7f, 269.5f, 180.2f );
		Tubulin tubulin;
		float lastReleaseTime = -1f;

		protected override bool canMove
		{
			get
			{
				return state == MotorState.Free;
			}
		}

		public override void DoRandomWalk ()
		{
			if (canMove)
			{
				Rotate();
				for (int i = 0; i < kinesin.maxIterations; i++)
				{
					if (Move() || state != MotorState.Free)
					{
						return;
					}
				}
				Jitter( 0.3f );
			}
			else 
			{
				Jitter();
			}
		}

		protected override void OnCollisionWithTubulin (Tubulin[] collidingTubulins)
		{
			if (Time.time - lastReleaseTime > 1f)
			{
				CheckForBind( collidingTubulins );
			}
		}

		protected override bool WithinLeash (Vector3 moveStep)
		{
			float d = Vector3.Distance( transform.parent.position, transform.position + moveStep );
			return d >= minDistanceFromParent && d <= maxDistanceFromParent;
		}

		void CheckForBind (Tubulin[] collidingTubulins)
		{
			List<Tubulin> tubulins = new List<Tubulin>();
			foreach (Tubulin t in collidingTubulins)
			{
				if (t.type == 1 && !t.hasMotorBound)
				{
					tubulins.Add( t );
				}
			}

			if (tubulins.Count > 0)
			{
				Tubulin t = FindClosestValidTubulin( tubulins );
				if (t != null)
				{
					BindTubulin( t );
				}
			}
		}

		Tubulin FindClosestValidTubulin (List<Tubulin> tubulins)
		{
			float d, hipsD, minDistance = Mathf.Infinity;
			Vector3 _bindingPosition;
			Tubulin closestTubulin = null;
			foreach (Tubulin t in tubulins)
			{
				_bindingPosition = t.transform.TransformPoint( bindingPosition );
				hipsD = Vector3.Distance( _bindingPosition, kinesin.hips.transform.position );
				d = Vector3.Distance( _bindingPosition, transform.position );
				if (hipsD <= maxDistanceFromParent && d < minDistance) // && closeToBindingOrientation( t )
				{
					minDistance = d;
					closestTubulin = t;
				}
			}
			return closestTubulin;
		}

		bool closeToBindingOrientation (Tubulin _tubulin)
		{
			Vector3 localRotation = (Quaternion.Inverse( _tubulin.transform.rotation ) * transform.rotation).eulerAngles;
			return Helpers.AngleIsWithinTolerance( localRotation.x, bindingRotation.x, bindingRotationTolerance )
				&& Helpers.AngleIsWithinTolerance( localRotation.y, bindingRotation.y, bindingRotationTolerance )
				&& Helpers.AngleIsWithinTolerance( localRotation.z, bindingRotation.z, bindingRotationTolerance );
		}

		void BindTubulin (Tubulin _tubulin)
		{
			state = MotorState.Weak;
			tubulin = _tubulin;
			tubulin.hasMotorBound = true;
			transform.rotation = tubulin.transform.rotation * Quaternion.Euler( bindingRotation );
			transform.position = tubulin.transform.TransformPoint( bindingPosition );
			kinesin.SetParentSchemeOnBind( this );
		}

		public void Release ()
		{
			state = MotorState.Free;
			lastReleaseTime = Time.time;
			if (tubulin != null)
			{
				tubulin.hasMotorBound = false;
				Vector3 fromTubulin = 1f * (transform.position - tubulin.transform.position).normalized;
				MoveIfValid( fromTubulin );
			}
			if (hipsAreLockedToThis)
			{
				kinesin.hips.SetFree();
				hipsAreLockedToThis = false;
			}
			ATP.SetActive( false );
		}

		public void BindATP ()
		{
			state = MotorState.Strong;
			ATP.SetActive( true );
		}

		public void BindNecklinker ()
		{
			hipsAreLockedToThis = true;
		}

		public override void Reset ()
		{
			state = MotorState.Free;
			hipsAreLockedToThis = false;
			tubulin = null;
			lastReleaseTime = -1f;
			ATP.SetActive( false );
		}
	}
}