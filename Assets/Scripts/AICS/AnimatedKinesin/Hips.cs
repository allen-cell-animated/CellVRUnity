using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.AnimatedKinesin
{
	public enum HipsState 
	{
		Free,
		Locked
	}

	public class Hips : Molecule 
	{
		public HipsState state = HipsState.Free;
		public float snapPosition = 4.5f; // nm in front of motor pivot
		public float snapSpeed = 30f; // degrees per second

		Transform secondParent = null;
		Vector3[] snappingArcPositions;
		int currentSnapStep = 0;
		bool snapping;
		float timePerSnapStep = 0.2f;
		float lastSnapStepTime = -1f;

		protected override bool canMove
		{
			get
			{
				return state != HipsState.Locked;
			}
		}

		public override void DoRandomWalk ()
		{
			if (canMove)
			{
				Rotate();
				for (int i = 0; i < kinesin.maxIterations; i++)
				{
					if (Move())
					{
						return;
					}
				}
				Jitter( 0.1f );
			}
			else
			{
				Jitter();
			}
		}

		public void StartSnap (Motor motor)
		{
			snappingArcPositions = CalculateSnapArcPositions( motor );

			motor.BindNecklinker();
			currentSnapStep = 0;
			snapping = true;
		}

		Vector3[] CalculateSnapArcPositions (Motor motor)
		{
			Vector3 snappedPosition = SnappedPosition( motor );
			Vector3 motorToCurrentPosition = transform.position - motor.transform.position;
			Vector3 motorToSnappedPosition = snappedPosition - motor.transform.position;
			float angle = (180f / Mathf.PI) * Mathf.Acos( Vector3.Dot( motorToCurrentPosition.normalized, motorToSnappedPosition.normalized ) );

			Vector3[] arcPositions = null;
			if (angle > 90f)
			{
				Vector3 motorToNorthPole = (motorToCurrentPosition.magnitude + motorToSnappedPosition.magnitude) / 2f * -motor.transform.up;
				float angleToNorthPole = (180f / Mathf.PI) * Mathf.Acos( Vector3.Dot( motorToCurrentPosition.normalized, motorToNorthPole.normalized ) );
				float angleNorthPoleToSnapped = (180f / Mathf.PI) * Mathf.Acos( Vector3.Dot( motorToNorthPole.normalized, motorToSnappedPosition.normalized ) );
				int steps = Mathf.RoundToInt( (angleToNorthPole + angleNorthPoleToSnapped) / (snapSpeed * timePerSnapStep) );

				Vector3[] arcPositions1 = CalculateArcPositions( motor.transform.position, motorToCurrentPosition, motorToNorthPole, 
					Mathf.RoundToInt( steps * angleToNorthPole / (angleToNorthPole + angleNorthPoleToSnapped) ) );
				Vector3[] arcPositions2 = CalculateArcPositions( motor.transform.position, motorToNorthPole, motorToSnappedPosition, 
					Mathf.RoundToInt( steps * angleNorthPoleToSnapped / (angleToNorthPole + angleNorthPoleToSnapped) ) );

				arcPositions = new Vector3[ arcPositions1.Length + arcPositions2.Length ];
				arcPositions1.CopyTo( arcPositions, 0 );
				arcPositions2.CopyTo( arcPositions, arcPositions1.Length );
			}
			else 
			{
				int steps = Mathf.RoundToInt( angle / (snapSpeed * timePerSnapStep) );
				arcPositions = CalculateArcPositions( motor.transform.position, motorToCurrentPosition, motorToSnappedPosition, steps );
			}
			return arcPositions;
		}

		Vector3[] CalculateArcPositions (Vector3 pivotPosition, Vector3 startLocalPosition, Vector3 goalLocalPosition, int steps)
		{
			float dLength = (goalLocalPosition.magnitude - startLocalPosition.magnitude) / steps;
			float dAngle = (180f / Mathf.PI) * Mathf.Acos( Vector3.Dot( startLocalPosition.normalized, goalLocalPosition.normalized ) ) / steps;
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

		Vector3 SnappedPosition (Motor strongMotor)
		{
			return strongMotor.transform.position + snapPosition * strongMotor.transform.forward;
		}

		public void UpdateSnap ()
		{
			if (snapping)
			{
				if (Time.time - lastSnapStepTime >= timePerSnapStep)
				{
					transform.position = snappingArcPositions[currentSnapStep];

					currentSnapStep++;
					if (currentSnapStep >= snappingArcPositions.Length)
					{
						state = HipsState.Locked;
						snapping = false;
					}

					lastSnapStepTime = Time.time;
				}
				else
				{
					DoRandomWalk();
				}
			}
		}

		public void SetFree ()
		{
			state = HipsState.Free;
		}

		protected override void OnCollisionWithTubulin (Tubulin[] collidingTubulins) { }

		public void SetSecondParent (Transform _secondParent)
		{
			secondParent = _secondParent;
		}

		protected override bool WithinLeash (Vector3 moveStep)
		{
			if (secondParent != null)
			{
				float d2 = Vector3.Distance( secondParent.position, transform.position + moveStep );
				if (d2 < minDistanceFromParent || d2 > maxDistanceFromParent)
				{
					return false;
				}
			}
			float d1 = Vector3.Distance( transform.parent.position, transform.position + moveStep );
			return d1 >= minDistanceFromParent && d1 <= maxDistanceFromParent;
		}

		public override void Reset ()
		{
			state = HipsState.Free;
			secondParent = null;
		}
	}
}