using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins.Kinesin
{
	public class Tropomyosin : LinkerComponentMolecule 
	{
		public Cargo cargo;
		public Transform previousTransform;
		public Hips hips;

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
			transform.rotation = Quaternion.LookRotation( previousTransform.position - cargo.transform.position );
		}

		void OnTriggerEnter (Collider other)
		{
			VelocityWatcher otherVelocity = other.GetComponent<VelocityWatcher>();
			if (otherVelocity != null)
			{
				cargo.Push( other.transform.position, otherVelocity.displacement );
			}
		}

		public override void DoRandomWalk () { }

		protected override void InteractWithBindingPartners () { }

		public override void DoCustomReset () { }
	}
}
