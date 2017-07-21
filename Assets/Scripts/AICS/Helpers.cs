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
	}
}