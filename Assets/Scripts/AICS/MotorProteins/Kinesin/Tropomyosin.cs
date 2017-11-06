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
		public float pushPower = 0.2f;
		public float ambientPower = 0.01f;

		float t = 0;
		bool bending = false;
		Quaternion startBendingRotation;
		Quaternion pushedRotation;
		Quaternion defaultRotation;

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
				if (bending)
				{
					transform.rotation = Quaternion.Slerp( startBendingRotation, pushedRotation, t );
				}
				else
				{
					transform.rotation = Quaternion.Slerp( pushedRotation, defaultRotation, t );
				}
			}
		}

		void OnTriggerEnter (Collider other)
		{
			if (!stickToCargo)
			{
				VelocityWatcher otherVelocity = other.GetComponent<VelocityWatcher>();
				if (otherVelocity != null)
				{
					StartLinkRotation( transform.position - otherVelocity.displacement, pushPower );
				}
			}
		}

		void RotateAmbiently ()
		{
			StartLinkRotation( transform.position + Helpers.GetRandomVector( 1f ), ambientPower );
		}

		void StartLinkRotation (Vector3 offset, float power)
		{
			Vector3 localOffset = transform.InverseTransformPoint( offset );
			startBendingRotation = transform.rotation;
			pushedRotation = Quaternion.LookRotation( transform.forward - power * new Vector3( localOffset.x, localOffset.y, 0 ).normalized );
			rotateDuration = Mathf.Abs( Quaternion.Angle( defaultRotation, pushedRotation ) ) / rotateSpeed;
			startRotatingNanoseconds = MolecularEnvironment.Instance.nanosecondsSinceStart;
			bending = true;
		}

		public override void DoRandomWalk () { }

		protected override void InteractWithBindingPartners () { }

		public override void DoCustomReset () { }
	}
}
