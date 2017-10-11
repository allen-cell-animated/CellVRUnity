using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public enum Direction
	{
		right, left,
		up, down,
		forward, backward
	}

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
			if (min >= max - 1f && min <= max + 1)
			{
				return true; // min and max are equal so all angles are within tolerance
			}
			else if (min > max)
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

		// Unchanged vector components should be < 0
		public static void SetJointRotationLimits (ConfigurableJoint joint, Vector3 newLimits)
		{
			for (int axis = 0; axis < 3; axis++)
			{
				if (newLimits[axis] >= 0)
				{
					if (newLimits[axis] < 5f)
					{
						SetJointAngularMotion( joint, axis, ConfigurableJointMotion.Locked );
					}
					else if (newLimits[axis] <= 177f)
					{
						SetJointAngularMotion( joint, axis, ConfigurableJointMotion.Limited );
						SetJointAngularLimit( joint, axis, newLimits[axis] );
					}
					else
					{
						SetJointAngularMotion( joint, axis, ConfigurableJointMotion.Free );
					}
				}
			}
		}

		public static void SetJointAngularMotion (ConfigurableJoint joint, int axis, ConfigurableJointMotion motion)
		{
			switch (axis)
			{
			case 0:
				joint.angularXMotion = motion;
				break;

			case 1:
				joint.angularYMotion = motion;
				break;

			case 2:
				joint.angularZMotion = motion;
				break;

			default:
				break;
			}
		}

		public static void SetJointAngularLimit (ConfigurableJoint joint, int axis, float limit)
		{
			SoftJointLimit jointLimit = new SoftJointLimit();
			jointLimit.limit = limit;

			switch (axis)
			{
			case 0:
				SoftJointLimit negativeJointLimit = new SoftJointLimit();
				negativeJointLimit.limit = -limit;

				joint.lowAngularXLimit = negativeJointLimit;
				joint.highAngularXLimit = jointLimit;
				break;

			case 1:
				joint.angularYLimit = jointLimit;
				break;

			case 2:
				joint.angularZLimit = jointLimit;
				break;

			default:
				break;
			}
		}

		public static void Shuffle<T> (this T[] array)
		{
			int n = array.Length;
			while (n > 1) 
			{ 
				int k = Random.Range( 0, n );
				n--;

				T value = array[k];  
				array[k] = array[n];  
				array[n] = value; 
			} 
		}

		public static int GetRandomIndex<T> (this T[] array)
		{
			return Mathf.CeilToInt( Random.Range( Mathf.Epsilon, 1f ) * array.Length ) - 1;
		}

		public static int GetExponentialRandomIndex<T> (this T[] array)
		{
			float i = Mathf.Clamp( -Mathf.Log10( Random.Range( Mathf.Epsilon, 1f ) ) / 2f, 0, 1f );
			return Mathf.CeilToInt( i * array.Length ) - 1;
		}

		public static void Shuffle<T> (this List<T> list)
		{
			int n = list.Count;
			while (n > 1) 
			{ 
				int k = Random.Range( 0, n );
				n--;

				T value = list[k];  
				list[k] = list[n];  
				list[n] = value; 
			} 
		}

		public static int GetRandomIndex<T> (this List<T> list)
		{
			return Mathf.CeilToInt( Random.Range( Mathf.Epsilon, 1f ) * list.Count ) - 1;
		}

		public static int GetExponentialRandomIndex<T> (this List<T> list)
		{
			float i = Mathf.Clamp( -Mathf.Log10( Random.Range( Mathf.Epsilon, 1f ) ) / 2f, 0, 1f );
			return Mathf.CeilToInt( i * list.Count ) - 1;
		}

		public static Vector3 GetLocalDirection (Direction direction, Transform transform)
		{
			switch (direction)
			{
			case Direction.right :
				return transform.right;

			case Direction.left :
				return -transform.right;

			case Direction.up :
				return transform.up;

			case Direction.down :
				return -transform.up;

			case Direction.forward :
				return transform.forward;

			case Direction.backward :
				return -transform.forward;

			default :
				return Vector3.zero;
			}
		}

		public static float SampleExponentialDistribution (float mean)
		{
			return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / mean);
		}

		public static float SampleNormalDistribution (float mean, float standardDeviation)
		{
			float n;
			float min = mean - 3.5f * standardDeviation;
			float max = mean + 3.5f * standardDeviation;

			do
			{
				n = mean + GetGaussian() * standardDeviation;
			} 
			while (n < min || n > max);

			return n;
		}

		public static float GetGaussian ()
		{
			float v1 = 0, v2 = 0, s = 0;
			while (s >= 1f || s == 0) 
			{
				v1 = 2f * Random.value - 1f;
				v2 = 2f * Random.value - 1f;
				s = v1 * v1 + v2 * v2;
			}
			s = Mathf.Sqrt( (-2f * Mathf.Log( s )) / s );
			return v1 * s;
		}
	}
}