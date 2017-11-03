using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins.Kinesin
{
	public class Tropomyosin : LinkerComponentMolecule 
	{
		public Cargo cargo;
		public Transform previousTransform;
		public bool stickToCargo;
		public float maxAngle = 25f;

		float t = 0;
		bool bending = false;
		Quaternion pushedRotation;
		Quaternion defaultRotation;
		Quaternion randomRotation;

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
			SetPosition( previousTransform.position );

			t = (MolecularEnvironment.Instance.nanosecondsSinceStart - startRotatingNanoseconds) / rotateDuration;
			defaultRotation = Quaternion.LookRotation( previousTransform.position - cargo.transform.position );
			if (!stickToCargo && (t < 1f || bending))
			{
				SetRotation();
			}
			else
			{
				RotateAmbiently();
			}
		}

		void SetRotation ()
		{
			if (t >= 1f)
			{
				if (bending)
				{
					startRotatingNanoseconds = MolecularEnvironment.Instance.nanosecondsSinceStart;
					bending = false;
				}
			}
			else
			{
				transform.rotation = Quaternion.Slerp( defaultRotation, pushedRotation, (bending ? t : 1f - t) );
			}
		}

		void OnTriggerEnter (Collider other)
		{
			if (!stickToCargo)
			{
				VelocityWatcher otherVelocity = other.GetComponent<VelocityWatcher>();
				if (otherVelocity != null)
				{
					StartLinkRotation( Vector3.ClampMagnitude( otherVelocity.velocity, maxAngle ) );
				}
			}
		}

		void StartLinkRotation (Vector3 offset)
		{
			pushedRotation = Quaternion.LookRotation( (previousTransform.position - offset) - cargo.transform.position );
			rotateDuration = Mathf.Abs( Quaternion.Angle( defaultRotation, pushedRotation ) ) / rotateSpeed;
			startRotatingNanoseconds = MolecularEnvironment.Instance.nanosecondsSinceStart;
			bending = true;
		}

		void RotateAmbiently ()
		{
			transform.rotation = defaultRotation;
//			transform.rotation = Quaternion.RotateTowards(  );
		}

		public override void DoRandomWalk () { }

		protected override void InteractWithBindingPartners () { }

		public override void DoCustomReset () { }
	}
}
