using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.MotorProteins
{
	// A basic molecule object
	public abstract class Molecule : MonoBehaviour 
	{
		public float radius;

		public abstract bool bound
		{
			get;
		}

		public abstract void Simulate ();

		public abstract void Reset ();
	}
}