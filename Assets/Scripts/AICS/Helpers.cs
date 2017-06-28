using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public static class Helpers 
	{
		// Get a vector of magnitude with random direction
		public static Vector3 GetRandomVector (float magnitude)
		{
			return magnitude * Random.onUnitSphere;
		}

		// Check if an angle is within a tolerance of another angle
		public static bool AngleIsWithinTolerance (float angle, float goal, float tolerance)
		{
			angle = ClampAngle( angle );
			float min = ClampAngle( ClampAngle( goal ) - ClampAngle( tolerance ) );
			float max = ClampAngle( ClampAngle( goal ) + ClampAngle( tolerance ) );
			if (min > max)
			{
				return !(angle < min && angle > max);
			}
			else 
			{
				return angle >= min && angle <= max;
			}
		}

		// Clamps angle between 0 and 360 (inclusive)
		public static float ClampAngle (float angle)
		{
			if (angle < 0f) 
			{
				return angle + (360f * (Mathf.Floor( -angle / 360f ) + 1f));
			}
			else if (angle > 360f)
			{
				return angle - (360f * Mathf.Floor( angle / 360f ));
			}
			else
			{
				return angle;
			}
		}
	}
}