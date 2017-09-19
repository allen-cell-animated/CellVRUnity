using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MotorProteins;

namespace AICS.Microtubule
{
	public class Tubulin : Molecule
	{
		public int tubulinType = -1;
		public bool hasMotorBound;

		public override bool bound
		{
			get
			{
				return true;
			}
		}

		protected override void OnAwake () { }

		public void Place (Vector3 position, Vector3 lookDirection, Vector3 normal)
		{
			transform.localPosition = position;
			transform.LookAt( transform.position + lookDirection, normal );
		}

		public override void DoCustomSimulation () { }

		public override void DoCustomReset () { }
	}
}