using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins.Kinesin
{
	public class Cargo : ComponentMolecule 
	{
		public Transform anchor;
		public float maxAngleFromUp = 30f;

		Kinesin kinesin
		{
			get
			{
				return assembly as Kinesin;
			}
		}

		public override bool bound
		{
			get
			{
				return false;
			}
		}

		protected override void OnAwake () { }

		public override void DoCustomSimulation ()
		{
			SnapToBounds();
			DoRandomWalk();
		}

		public override void DoRandomWalk () 
		{
			int i = 0;
			bool retry = false;
			bool success = false;
			while (!success && i < MolecularEnvironment.Instance.maxIterationsPerStep)
			{
				success = Move( retry );
				retry = true;
				i++;
			}
		}

		protected override void InteractWithBindingPartners () { }

		public override void DoCustomReset () { }

		void SnapToBounds ()
		{
			ClampDistance();
			ClampAngle();
		}

		void ClampDistance ()
		{
			Vector3 anchorToPosition = transform.position - anchor.position;
			SetPosition( anchor.position + Mathf.Clamp( anchorToPosition.magnitude, minDistanceFromParent, maxDistanceFromParent ) * anchorToPosition.normalized );
		}

		void ClampAngle ()
		{
			if (kinesin.lastTubulin != null)
			{
				Vector3 upFromAnchor = kinesin.lastTubulin.transform.up;
				Vector3 anchorToPosition = transform.position - anchor.position;
				float angle = Mathf.Acos( Vector3.Dot( upFromAnchor, anchorToPosition.normalized ) );
				if (180f * angle / Mathf.PI > maxAngleFromUp)
				{
					Vector3 axis = Vector3.Cross( upFromAnchor, anchorToPosition.normalized ).normalized;
					Vector3 goalPosition = anchor.position + anchorToPosition.magnitude * (Quaternion.AngleAxis( angle, axis ) * upFromAnchor);
					transform.position += 3f * meanStepSize * (goalPosition - transform.position).normalized;
				}
			}
		}

		protected override bool CheckLeash (Vector3 moveStep)
		{
			if (DistanceFromAnchorIsWithinBounds( moveStep ))
			{
				if (kinesin.lastTubulin == null)
				{
					return true;
				}
				return AngleFromCenter( moveStep ) <= maxAngleFromUp;
			}
			return false;
		}

		bool DistanceFromAnchorIsWithinBounds (Vector3 moveStep)
		{
			float d = Vector3.Distance( anchor.position, transform.position + moveStep );
			return d >= minDistanceFromParent && d <= maxDistanceFromParent;
		}

		float AngleFromCenter (Vector3 moveStep)
		{
			Vector3 upFromAnchor = kinesin.lastTubulin.transform.up;
			Vector3 anchorToNewPosition = (transform.position + moveStep - anchor.position).normalized;
			return 180f * Mathf.Acos( Vector3.Dot( upFromAnchor, anchorToNewPosition ) ) / Mathf.PI;
		}
	}
}
