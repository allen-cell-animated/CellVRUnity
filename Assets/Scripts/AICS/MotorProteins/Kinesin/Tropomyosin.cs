using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins.Kinesin
{
	public class Tropomyosin : LinkerComponentMolecule 
	{
		public Hips hips;
		public Cargo cargo;

		CapsuleCollider _capsuleCollider;
		CapsuleCollider capsuleCollider
		{
			get 
			{
				if (_capsuleCollider == null)
				{
					_capsuleCollider = GetComponent<CapsuleCollider>();
				}
				return _capsuleCollider;
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
			PositionBetweenHipsAndCargo();
		}

		void PositionBetweenHipsAndCargo ()
		{
			Vector3 hipsToCargo = cargo.transform.position - hips.transform.position;
			float length = hipsToCargo.magnitude - cargo.radius;

			//position
			SetPosition( hips.transform.position + (hips.radius + (length / 2f)) * hipsToCargo.normalized );

			//rotation
			transform.LookAt( hips.transform );

			//scale
//			transform.GetChild( 0 ).localScale = new Vector3( 1f, length / 2f, 1f );
			capsuleCollider.height = length;
		}

		public override void DoRandomWalk () { }

		protected override void InteractWithBindingPartners () { }

		public override void DoCustomReset () { }
	}
}
