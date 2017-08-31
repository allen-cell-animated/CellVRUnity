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
			DoRandomWalk();
		}

		public override void DoRandomWalk () 
		{ 
			Move();		
		}

		protected override void InteractWithCollidingMolecules () { }

		public override void DoCustomReset () { }

		protected override bool CheckLeash (Vector3 moveStep)
		{
			if (kinesin.lastTubulin != null)
			{
				float d = Vector3.Distance( anchor.position, transform.position + moveStep );
				if (d >= minDistanceFromParent && d <= maxDistanceFromParent)
				{
					Vector3 upFromAnchor = kinesin.lastTubulin.transform.up;
					Vector3 anchorToNewPosition = (transform.position + moveStep - anchor.position).normalized;
					float angle = 180f * Mathf.Acos( Vector3.Dot( upFromAnchor, anchorToNewPosition ) ) / Mathf.PI;

					return angle <= maxAngleFromUp;
				}
			}
			return true;
		}
	}
}
