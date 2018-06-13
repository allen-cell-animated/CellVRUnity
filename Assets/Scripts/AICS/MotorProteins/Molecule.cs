using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		public MoleculeType[] typesToIgnoreCollisionsWith;

		public MeshRenderer[] meshRenderers;
		protected Color color;

		public bool ShouldIgnoreCollision (Molecule other)
		{
			if (typesToIgnoreCollisionsWith == null)
			{
				return false;
			}
			foreach (MoleculeType t in typesToIgnoreCollisionsWith)
			{
				if (other.type == t)
				{
					return true;
				}
			}
			return false;
		}

		public void Flash (Color _flashColor)
		{
			foreach (MeshRenderer meshRenderer in meshRenderers)
			{
				if (meshRenderer != null) 
				{ 
					meshRenderer.material.color = _flashColor;
				}
			}
			Invoke( "EndFlash", 0.5f );
		}

		void EndFlash ()
		{
			foreach (MeshRenderer meshRenderer in meshRenderers)
			{
				if (meshRenderer != null) 
				{ 
					meshRenderer.material.color = color;
				}
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
			if (meshRenderers.Length > 0 && meshRenderers[0] != null) { color = meshRenderers[0].material.color; }
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
            SetPosition( ClampedPosition( transform.position + moveStep ) );
		}

        protected virtual Vector3 ClampedPosition (Vector3 newPosition)
        {
            return newPosition;
        }

		protected void SetPosition (Vector3 newPosition)
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

		public void DoReset ()
		{
			transform.position = startPosition;
			transform.rotation = startRotation;
			resetFrames = 3;

			DoCustomReset();
		}

		public abstract void DoCustomReset ();
	}
}