using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins
{
	public enum CollisionDetectionMethod
	{
		Sweeptest,
		Spheres
	}

	public class MolecularEnvironment : MonoBehaviour 
	{
		public float nanosecondsPerStep = 1E5f;
		public int stepsPerFrame = 1;
		public int maxIterationsPerStep = 50;
		public CollisionDetectionMethod collisionDetectionMethod;
		public LayerMask[] moleculeLayers;

		static MolecularEnvironment _Instance;
		public static MolecularEnvironment Instance
		{
			get
			{
				if (_Instance == null)
				{
					_Instance = GameObject.FindObjectOfType<MolecularEnvironment>();
				}
				return _Instance;
			}
		}

		Molecule[] _molecules;
		Molecule[] molecules
		{
			get
			{
				if (_molecules == null)
				{
					_molecules = GameObject.FindObjectsOfType<Molecule>();
				}
				return _molecules;
			}
		}

		void Start ()
		{
			SetCollisionDetectionMethod( collisionDetectionMethod );
		}

		public void SetCollisionDetectionMethod (CollisionDetectionMethod _collisionDetectionMethod)
		{
			collisionDetectionMethod = _collisionDetectionMethod;

			SetCollisionsBetweenMoleculeLayers( collisionDetectionMethod == CollisionDetectionMethod.Spheres );
			SetMoleculeDetectorsActive( collisionDetectionMethod == CollisionDetectionMethod.Spheres );
		}

		void SetCollisionsBetweenMoleculeLayers (bool enabled)
		{
			for (int i = 0; i < moleculeLayers.Length; i++)
			{
				for (int j = i + 1; j < moleculeLayers.Length; j++)
				{
					Physics.IgnoreLayerCollision( Mathf.RoundToInt( Mathf.Log( moleculeLayers[i], 2f ) ), 
						Mathf.RoundToInt( Mathf.Log( moleculeLayers[j], 2f ) ), enabled );
				}
			}
		}

		void SetMoleculeDetectorsActive (bool active)
		{
			foreach (Molecule molecule in molecules)
			{
				molecule.SetMoleculeDetectorsActive( active );
			}
		}
	}
}