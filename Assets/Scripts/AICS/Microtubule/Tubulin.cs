﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MotorProteins;

namespace AICS.Microtubule
{
	public class Tubulin : Molecule
	{
		public int type = -1;
		public bool hasMotorBound;

		public override bool bound
		{
			get
			{
				return true;
			}
		}

		public void Place (Vector3 position, Vector3 lookDirection, Vector3 normal)
		{
			transform.localPosition = position;
			transform.LookAt( transform.position + lookDirection, normal );
		}

		public override void Simulate () { }

		public override void Reset () { }
	}
}