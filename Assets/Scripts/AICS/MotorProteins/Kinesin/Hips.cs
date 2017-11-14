﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins.Kinesin
{
	public enum HipsState 
	{
		Free,
		Locked
	}

	public class Hips : LinkerComponentMolecule 
	{
		public bool frozen = false;
		public HipsState state = HipsState.Free;
		public float snapPosition = 5.5f; // nm in front of motor pivot
		public float snapSpeed = 9000f; // degrees per simulated second
		public bool doSnap = true;

		Vector3[] snappingArcPositions;
		int currentSnapStep = 0;
		public bool snapping;
		public Motor lastSnappingPivot;
		float StartMeanStepSize;
		float degreesPerSnapStep = 30f;

		public Kinesin kinesin
		{
			get
			{
				return assembly as Kinesin;
			}
		}

		public override bool bound
		{
			get
			{
				return state == HipsState.Locked;
			}
		}

		protected override void OnAwake () 
		{
			StartMeanStepSize = meanStepSize;
		}

		public override void DoCustomSimulation ()
		{
			if (!frozen)
			{
				DoRandomWalk();
			}
		}

		protected override void InteractWithBindingPartners () { }

		// --------------------------------------------------------------------------------------------------- Random walk

		public override void DoRandomWalk ()
		{
			if (!bound)
			{
				RotateRandomly();

				int i = 0;
				bool retry = false;
				bool success = false;
				while (!success && i < 3f * MolecularEnvironment.Instance.maxIterationsPerStep)
				{
					success = MoveRandomly( retry );
					retry = true;
					i++;
				}

				if (!success)
				{
					Jitter( 0.1f );
				}
			}
			else
			{
				Jitter();
			}
		}

		// --------------------------------------------------------------------------------------------------- Snap

		public void StartSnap (Motor motor)
		{
			UnityEngine.Profiling.Profiler.BeginSample( "HipsSnap" );
			if (doSnap && !(motor == lastSnappingPivot && state == HipsState.Locked))
			{
				snappingArcPositions = CalculateSnapArcPositions( motor );
				foreach (Vector3 pos in snappingArcPositions)
				{
					GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = pos;
				}
				UnityEditor.EditorApplication.isPaused = true;
				lastSnappingPivot = motor;
				currentSnapStep = 0;
				state = HipsState.Free;
				snapping = true;
				meanStepSize = 0.5f * StartMeanStepSize;
				MoveTo( snappingArcPositions[0], true );
			}
			UnityEngine.Profiling.Profiler.EndSample();
		}

		Vector3[] CalculateSnapArcPositions (Motor motor)
		{
			Vector3 snappedPosition = SnappedPosition( motor );
			if (Vector3.Distance( snappedPosition, transform.position ) > 1f)
			{
				Vector3 motorToCurrentPosition = transform.position - motor.transform.position;
				Vector3 motorToSnappedPosition = snappedPosition - motor.transform.position;
				float angle = (180f / Mathf.PI) * Mathf.Acos( Mathf.Clamp( Vector3.Dot( motorToCurrentPosition.normalized, motorToSnappedPosition.normalized ), -1f, 1f ) );

				Vector3[] arcPositions = null;
				if (angle > 90f)
				{
					Vector3 motorToNorthPole = (motorToCurrentPosition.magnitude + motorToSnappedPosition.magnitude) / 2f * motor.transform.up;

					float angleToNorthPole = Mathf.Rad2Deg * Mathf.Acos( Mathf.Clamp( Vector3.Dot( motorToCurrentPosition.normalized, motorToNorthPole.normalized ), -1f, 1f ) );
					Vector3[] arcPositions1 = CalculateArcPositions( motor.transform.position, motorToCurrentPosition, motorToNorthPole, 
						Mathf.Max( Mathf.RoundToInt( angleToNorthPole / degreesPerSnapStep ), 1 ) );
					
					float angleNorthPoleToSnapped = Mathf.Rad2Deg * Mathf.Acos( Mathf.Clamp( Vector3.Dot( motorToNorthPole.normalized, motorToSnappedPosition.normalized ), -1f, 1f ) );
					Vector3[] arcPositions2 = CalculateArcPositions( motor.transform.position, motorToNorthPole, motorToSnappedPosition, 
						Mathf.Max( Mathf.RoundToInt( angleNorthPoleToSnapped / degreesPerSnapStep ), 1 ) );

					arcPositions = new Vector3[ arcPositions1.Length + arcPositions2.Length ];
					arcPositions1.CopyTo( arcPositions, 0 );
					arcPositions2.CopyTo( arcPositions, arcPositions1.Length );
				}
				else 
				{
					int steps = Mathf.Max( Mathf.RoundToInt( angle / degreesPerSnapStep ), 1 );
					arcPositions = CalculateArcPositions( motor.transform.position, motorToCurrentPosition, motorToSnappedPosition, steps );
				}
				return arcPositions;
			}
			else 
			{
				return new Vector3[1] {snappedPosition};
			}
		}

		Vector3 SnappedPosition (Motor strongMotor)
		{
			return strongMotor.transform.position + Mathf.Min( snapPosition, maxDistanceFromParent ) * -strongMotor.transform.right;
		}

		Vector3[] CalculateArcPositions (Vector3 pivotPosition, Vector3 startLocalPosition, Vector3 goalLocalPosition, int steps)
		{
			float dLength = (goalLocalPosition.magnitude - startLocalPosition.magnitude) / steps;
			float dAngle = (180f / Mathf.PI) * Mathf.Acos( Mathf.Clamp( Vector3.Dot( startLocalPosition.normalized, goalLocalPosition.normalized ), -1f, 1f ) ) / steps;
			Vector3 axis = Vector3.Cross( startLocalPosition.normalized, goalLocalPosition.normalized ).normalized;
			Vector3[] arcPositions = new Vector3[steps];

			for (int i = 0; i < steps - 1; i++)
			{
				arcPositions[i] = pivotPosition + (startLocalPosition.magnitude + (i + 1f) * dLength) 
					* (Quaternion.AngleAxis( (i + 1f) * dAngle, axis ) * startLocalPosition.normalized);
			}
			arcPositions[arcPositions.Length - 1] = pivotPosition + goalLocalPosition;
			return arcPositions;
		}

		protected override void OnFinishMove () 
		{
			if (snapping)
			{
				Debug.Log( currentSnapStep + " / " + snappingArcPositions.Length );
				if (currentSnapStep + 1 < snappingArcPositions.Length)
				{
					currentSnapStep++;
					MoveTo( snappingArcPositions[currentSnapStep], true );
				}
				else
				{
					snapping = moving = false;
				}
			}
		}

		public void SetFree (Motor releasedMotor)
		{
			if (lastSnappingPivot == releasedMotor)
			{
				snapping = moving = false;
				meanStepSize = StartMeanStepSize;
				state = HipsState.Free;
			}
		}

		// --------------------------------------------------------------------------------------------------- Reset

		public override void DoCustomReset ()
		{
			state = HipsState.Free;
			secondParent = null;
		}
	}
}