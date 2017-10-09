using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.MotorProteins
{
	// A basic molecule object
	public abstract class Molecule : MonoBehaviour 
	{
		public bool logEvents;
		public MoleculeType type;
		public float radius;
		public MoleculeDetector[] moleculeDetectors;
		public Vector3 startPosition;
		public Quaternion startRotation;
		public int resetFrames;

		public MeshRenderer meshRenderer;
		Color color;

		public void Flash (Color _flashColor)
		{
			if (meshRenderer != null) 
			{ 
				meshRenderer.material.color = _flashColor;
				Invoke( "EndFlash", 1.5f );
			}
		}

		void EndFlash ()
		{
			if (meshRenderer != null) 
			{ 
				meshRenderer.material.color = color;
			}
		}

		public abstract bool bound
		{
			get;
		}

		void Awake ()
		{
			startPosition = transform.position;
			startRotation = transform.rotation;
			if (meshRenderer != null) { color = meshRenderer.material.color; }
			OnAwake();
		}

		protected abstract void OnAwake ();

		public void Simulate ()
		{
			if (!MolecularEnvironment.Instance.pause)
			{
				DoCustomSimulation();
			}
		}

		public abstract void DoCustomSimulation ();

		protected void IncrementPosition (Vector3 moveStep)
		{
			SetPosition( transform.position + moveStep );
		}

		public void SetPosition (Vector3 newPosition)
		{
			transform.position = (resetFrames > 0) ? startPosition : newPosition;
		}

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
			resetFrames = 3;

			DoCustomReset();
		}

		public abstract void DoCustomReset ();
	}
}