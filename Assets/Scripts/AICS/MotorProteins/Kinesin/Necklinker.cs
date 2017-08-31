using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins.Kinesin
{
	public class Necklinker : LinkerComponentMolecule 
	{
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
			Jitter( 0.1f );
			PositionBetweenParents();
		}

		void PositionBetweenParents ()
		{
			if (transform.parent != null && secondParent != null)
			{
				transform.position = (transform.parent.position + secondParent.position) / 2f;
			}
		}

		public override void DoRandomWalk () { }

		protected override void InteractWithCollidingMolecules () { }

		public override void DoCustomReset () { }
	}
}
