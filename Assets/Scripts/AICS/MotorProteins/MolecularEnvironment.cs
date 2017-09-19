using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins
{
	public class MolecularEnvironment : MonoBehaviour 
	{
		public bool pause;
		public float timeMultiplier = 300f;
		public float nanosecondsPerStep = 1E5f;
		public float idealFrameRate = 30f;
		public int stepsPerFrame = 1;
		public int maxIterationsPerStep = 50;
		public Vector3 size = 100f * Vector3.one;
		public float gridSize = 50f;
		public LayerMask resolutionManagementLayer;
		public float[] LODDistances;
		public float nanosecondsSinceStart;
		public int stepsSinceStart;

		float averageFrameRate;
		int frameRateSamples;
		float startTime = 0;

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

		void OnDrawGizmos ()
		{
			Gizmos.DrawWireCube( transform.position, size );
		}

		void Start ()
		{
			CreateResolutionNodes();
		}

		void Update ()
		{
			CalculateFrameRate();

			for (int i = 0; i < stepsPerFrame; i++)
			{
				if (!pause)
				{
					nanosecondsSinceStart += nanosecondsPerStep;
					stepsSinceStart++;
				}
			}
		}

		// --------------------------------------------------------------------------------------------------- Resolution

		void CreateResolutionNodes ()
		{
			Vector3 divisions = size / gridSize;
			Vector3 position = -(size - gridSize * Vector3.one) / 2f;
			for (int x = 0; x < Mathf.FloorToInt( divisions.x ); x++)
			{
				for (int y = 0; y < Mathf.FloorToInt( divisions.y ); y++)
				{
					for (int z = 0; z < Mathf.FloorToInt( divisions.z ); z++)
					{
						CreateResolutionNode( position, x + "_" + y + "_" + z );
						position += gridSize * Vector3.forward;
					}
					position += gridSize * Vector3.up - size.z * Vector3.forward;
				}
				position += gridSize * Vector3.right - size.y * Vector3.up;
			}
		}

		void CreateResolutionNode (Vector3 position, string _name)
		{
			ResolutionNode node = GameObject.CreatePrimitive( PrimitiveType.Cube ).AddComponent<ResolutionNode>();
			node.transform.SetParent( transform );
			node.transform.localPosition = position;
			node.gameObject.layer = Mathf.RoundToInt( Mathf.Log( resolutionManagementLayer, 2f ) );
			node.name = "node_" + _name;
			node.Setup( this );
		}

		// --------------------------------------------------------------------------------------------------- Time

		public void SetTime (float _timeMultiplier)
		{
			timeMultiplier = _timeMultiplier;
			UpdateTimePerStep();
		}

		void CalculateFrameRate ()
		{
			averageFrameRate = (averageFrameRate * frameRateSamples + 1f / Time.deltaTime) / (frameRateSamples + 1f);
			frameRateSamples++;

			UpdateStepsPerFrame();
		}

		void UpdateStepsPerFrame ()
		{
			if (frameRateSamples >= 50)
			{
				int a = -Mathf.FloorToInt( (idealFrameRate - averageFrameRate) / (0.1f * idealFrameRate) );
				if (a != 0)
				{
					stepsPerFrame = Mathf.Max( 1, stepsPerFrame + Mathf.Clamp( a, -5, 5 ) );
					UpdateTimePerStep();
					averageFrameRate = 1f / Time.deltaTime;
					frameRateSamples = 1;
				}
			}
		}

		void UpdateTimePerStep ()
		{
			nanosecondsPerStep = 1E9f * Time.deltaTime / (timeMultiplier * stepsPerFrame);
		}

		public void Reset ()
		{
			nanosecondsSinceStart = 0;
			stepsSinceStart = 0;
			startTime = Time.time;
		}

		public float timeSinceRestart
		{
			get
			{
				return Time.time - startTime;
			}
		}
	}
}