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
		public CollisionDetectionMethod collisionDetectionMethod;
		public int maxIterationsPerStep = 50;

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
	}
}