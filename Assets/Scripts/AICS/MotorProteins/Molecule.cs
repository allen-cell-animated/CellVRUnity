using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.MotorProteins
{
	// A basic molecule object
	public abstract class Molecule : MonoBehaviour 
	{
		public float nanosecondsSinceStart;
		public int stepsSinceStart;
		public float radius;
		public MoleculeDetector[] moleculeDetectors;
		public Vector3 startPosition;
		public Quaternion startRotation;

		public abstract bool bound
		{
			get;
		}

		void Awake ()
		{
			startPosition = transform.position;
			startRotation = transform.rotation;
			OnAwake();
		}

		protected abstract void OnAwake ();

		public void Simulate ()
		{
			if (!MolecularEnvironment.Instance.pause)
			{
				DoCustomSimulation();

				nanosecondsSinceStart += MolecularEnvironment.Instance.nanosecondsPerStep;
				stepsSinceStart++;
			}
		}

		public abstract void DoCustomSimulation ();

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

		public void Reset ()
		{
			transform.position = startPosition;
			transform.rotation = startRotation;

			DoCustomReset();

			nanosecondsSinceStart = 0;
			stepsSinceStart = 0;
		}

		public abstract void DoCustomReset ();
	}
}