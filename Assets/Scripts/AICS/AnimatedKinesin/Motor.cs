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

		Vector3 bindingPosition = new Vector3( 0.34f, 4.01f, 0.34f );
		Vector3 bindingRotation = new Vector3( 357.7f, 269.5f, 180.2f );

		Kinesin _kinesin;
		Kinesin kinesin
		{
			get
			{
				if (_kinesin == null)
				{
					_kinesin = GameObject.FindObjectOfType<Kinesin>();
				}
				return _kinesin;
			}
		}

		void LateUpdate () 
		{
			if (state == MotorState.Free)
			{
				Move();
				Rotate();
			}
			else if (state == MotorState.Weak)
			{
				Jitter();
			}
		}

		protected override bool WillCollide (Vector3 moveStep)
		{
			RaycastHit[] hits = body.SweepTestAll( moveStep.normalized, moveStep.magnitude, UnityEngine.QueryTriggerInteraction.Collide );
			if (hits.Length > 0)
			{
				CheckHitsForTubulin( hits );
				return true;
			}
			return false;
		}

		protected override bool WithinLeash (Vector3 moveStep)
		{
			return Vector3.Distance( transform.parent.position, transform.position + moveStep ) <= maxDistanceFromParent;
		}

		protected override Vector3 towardLeashDirection
		{
			get
			{
				return (transform.parent.position - transform.position).normalized;
			}
		}

		void CheckHitsForTubulin (RaycastHit[] hits)
		{
			Tubulin tubulin;
			List<Tubulin> tubulins = new List<Tubulin>();
			foreach (RaycastHit hit in hits)
			{
				tubulin = hit.collider.GetComponent<Tubulin>();
				if (tubulin != null && tubulin.type == 1 && !tubulin.hasMotorBound)
				{
					tubulins.Add( tubulin );
				}
			}

			if (tubulins.Count > 0)
			{
				tubulin = FindClosestValidTubulin( tubulins );
				if (tubulin != null)
				{
					BindTubulin( tubulin );
				}
			}
		}

		Tubulin FindClosestValidTubulin (List<Tubulin> tubulins)
		{
			float d, hipsD, minDistance = Mathf.Infinity;
			Vector3 _bindingPosition;
			Tubulin closestTubulin = null;
			foreach (Tubulin tubulin in tubulins)
			{
				_bindingPosition = tubulin.transform.TransformPoint( bindingPosition );
				hipsD = Vector3.Distance( _bindingPosition, kinesin.hips.transform.position );
				d = Vector3.Distance( _bindingPosition, transform.position );
				if (hipsD <= maxDistanceFromParent && closeToBindingOrientation( tubulin ) && d < minDistance)
				{
					minDistance = d;
					closestTubulin = tubulin;
				}
			}
			return closestTubulin;
		}

		bool closeToBindingOrientation (Tubulin tubulin)
		{
			Vector3 localRotation = (Quaternion.Inverse( tubulin.transform.rotation ) * transform.rotation).eulerAngles;
			return Helpers.AngleIsWithinTolerance( localRotation.x, bindingRotation.x, bindingRotationTolerance )
				&& Helpers.AngleIsWithinTolerance( localRotation.y, bindingRotation.y, bindingRotationTolerance )
				&& Helpers.AngleIsWithinTolerance( localRotation.z, bindingRotation.z, bindingRotationTolerance );
		}

		void BindTubulin (Tubulin tubulin)
		{
			state = MotorState.Weak;
			tubulin.hasMotorBound = true;
			transform.rotation = tubulin.transform.rotation * Quaternion.Euler( bindingRotation );
			transform.position = tubulin.transform.TransformPoint( bindingPosition );
			kinesin.BindMotor( this );
		}

		public void Release ()
		{
			state = MotorState.Free;
		}
	}
}