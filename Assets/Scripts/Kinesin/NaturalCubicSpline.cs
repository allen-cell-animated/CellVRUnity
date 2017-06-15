using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class NaturalCubicSpline : Spline 
	{
		Vector3[] segmentPositions;
		Vector3[] tangents;

		bool needToCalculateSegments
		{
			get {
				return segmentPositions == null || segmentPositions.Length < 2 || pointsChanged;
			}
		}

		void CheckCalculateSegments ()
		{
			if (needToCalculateSegments)
			{
				CalculateSegments();
			}
		}

		void CalculateSegments ()
		{
			
		}

		public override Vector3 GetPoint (float t)
		{
			CheckCalculateSegments();
			return Vector3.zero;
		}

		public override float GetTForClosestPoint (Vector3 point) 
		{
			CheckCalculateSegments();
			return 0;
		}

		public override Vector3 GetNormal (float t)
		{
			CheckCalculateSegments();
			return Vector3.zero;
		}

		public override Vector3 GetTangent (float t)
		{
			CheckCalculateSegments();
			return Vector3.zero;
		}
	}
}