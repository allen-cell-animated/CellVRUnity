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
		public MoleculeDetector[] moleculeDetectors;

		public abstract bool bound
		{
			get;
		}

		public abstract void Simulate ();

		public void SetMoleculeDetectorsActive (bool active)
		{
			foreach (MoleculeDetector detector in moleculeDetectors)
			{
				detector.gameObject.SetActive( active );
			}
		}

		public virtual bool OtherWillCollide (Molecule other, Vector3 moveStep)
		{
			return Vector3.Distance( transform.position, other.transform.position + moveStep ) <= radius + other.radius;
		}

		public abstract void Reset ();
	}
}