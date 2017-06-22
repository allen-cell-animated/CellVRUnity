using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	[System.Serializable]
	public class CubicSplinePoint
	{
		public Vector3 position;
		public Vector3 tangent;
		public Quaternion rotation;
		public float arcLength;

		public CubicSplinePoint (Vector3 _position, Vector3 _tangent, Quaternion _rotation, float _arcLength)
		{
			position = _position;
			tangent = _tangent;
			rotation = _rotation;
			arcLength = _arcLength;
		}
	}

	public class CubicSplinePosition
	{
		public int pointIndex;
		public float sectionT;

		public CubicSplinePosition (int _pointIndex, float _sectionT)
		{
			pointIndex = _pointIndex;
			sectionT = _sectionT;
		}
	}

	public class NaturalCubicSpline : Spline 
	{
		public CubicSplinePoint[] calculatedPoints;

		// ---------------------------------------------- Length

		public override float GetLength ()
		{
			if (haveNotCalculated)
			{
				CalculateCurve();
			}
			float l = 0;
			for (int i = 1; i < calculatedPoints.Length; i++)
			{
				l += calculatedPoints[i].arcLength;
			}
			return l;
		}

		// ---------------------------------------------- Drawing

		protected override void UpdateCurve ()
		{
			CalculateCurve();
		}

		protected override void Draw ()
		{
			for (int i = 0; i < segmentPositions.Length - 1; i++)
			{
				DrawSegment( i, segmentPositions[i], segmentPositions[i + 1] );
			}
		}

		// ---------------------------------------------- Calculation

		bool haveNotCalculated 
		{
			get
			{
				return calculatedPoints == null || calculatedPoints.Length == 0;
			}
		}

		float[][] linearSystemSolution;

		ArbitraryMatrix coefficientMatrix
		{
			get 
			{
				ArbitraryMatrix a = new ArbitraryMatrix( n, n );
				for (int r = 1; r < n - 1; r++)
				{
					a[r][r] = 4f;
					a[r][r-1] = a[r][r+1] = 1f;
				}
				a[0][0] = a[n-1][n-1] = 1;
				return a;
			}
		}

		int n 
		{
			get
			{
				return points.Length;
			}
		}

		void CalculateCurve ()
		{
			linearSystemSolution = SolveLinearSystem();

			int divisions = Mathf.FloorToInt( (float) resolution / (float) n );
			segmentPositions = new Vector3[divisions * (n - 1) + n];
			calculatedPoints = new CubicSplinePoint[segmentPositions.Length];
			int k = 0;
			Vector3 tangent;
			normalTransform.rotation = Quaternion.identity;
			float arcLength = 0, t = 0;
			for (int section = 0; section < n - 1; section++)
			{
				int segments = (section < n - 2 ? divisions + 1 : divisions + 2);
				for (int s = 0; s < segments; s++)
				{
					t = s / (divisions + 1f);
					segmentPositions[k] = CalculatePosition( section, t );
					tangent = CalculateTangent( section, t );
					if (k > 0)
					{
						arcLength = Vector3.Distance( segmentPositions[k], segmentPositions[k - 1] );
					}
					calculatedPoints[k] = new CubicSplinePoint( segmentPositions[k], tangent, CalculateRotation( segmentPositions[k], tangent ), arcLength );
					k++;
				}
			}
		}

		float[][] SolveLinearSystem ()
		{
			float[][] solution = new float[3][];
			for (int axis = 0; axis < 3; axis++)
			{
				float[] b = new float[n];
				for (int i = 1; i < n - 1; i++)
				{
					b[i] = points[i + 1].position[axis] - 2f * points[i].position[axis] + points[i - 1].position[axis];
				}
				solution[axis] = coefficientMatrix.inverse * b;
			}
			return solution;
		}

		Vector3 CalculatePosition (int pointIndex, float sectionT)
		{
			Vector3 position = Vector3.zero;
			for (int axis = 0; axis < 3; axis++)
			{
				float ct = points[pointIndex + 1].position[axis] - linearSystemSolution[axis][pointIndex + 1];
				float c1t = points[pointIndex].position[axis] - linearSystemSolution[axis][pointIndex];
				position[axis] = linearSystemSolution[axis][pointIndex + 1] * Mathf.Pow( sectionT, 3f ) 
					+ linearSystemSolution[axis][pointIndex] * Mathf.Pow( 1f - sectionT, 3f ) + ct * sectionT + c1t * (1f - sectionT);
			}
			return position;
		}

		Vector3 CalculateTangent (int pointIndex, float sectionT)
		{
			Vector3 tangent = Vector3.zero;
			for (int axis = 0; axis < 3; axis++)
			{
				float ct = points[pointIndex + 1].position[axis] - linearSystemSolution[axis][pointIndex + 1];
				float c1t = points[pointIndex].position[axis] - linearSystemSolution[axis][pointIndex];
				tangent[axis] = 3f * linearSystemSolution[axis][pointIndex + 1] * Mathf.Pow( sectionT, 2f ) 
					- 3f * linearSystemSolution[axis][pointIndex] * Mathf.Pow( 1f - sectionT, 2f ) + ct - c1t;
			}
			return Vector3.Normalize( tangent );
		}

		Quaternion CalculateRotation (Vector3 position, Vector3 tangent)
		{
			float angle = 180f * Mathf.Acos( Vector3.Dot( normalTransform.forward, tangent ) ) / Mathf.PI;
			Vector3 axis = Vector3.Normalize( Vector3.Cross( normalTransform.forward, tangent ) );
			normalTransform.RotateAround( position, axis, angle );
			return normalTransform.rotation;
		}

		CubicSplinePosition GetSplinePositionForT (float t)
		{
			float tLength = t * length;
			float currentLength = 0;
			for (int i = 1; i < calculatedPoints.Length; i++)
			{
				float arcLength = calculatedPoints[i].arcLength;
				if (tLength < currentLength + arcLength)
				{
					return new CubicSplinePosition( i - 1, (tLength - currentLength) / arcLength );
				}
				currentLength += arcLength;
			}
			return new CubicSplinePosition( calculatedPoints.Length - 2, 1f );
		}

		public override Vector3 GetPoint (float t)
		{
			CubicSplinePosition splinePosition = GetSplinePositionForT( t );
			Vector3 startPosition = calculatedPoints[splinePosition.pointIndex].position;
			Vector3 endPosition = calculatedPoints[splinePosition.pointIndex + 1].position;
			return Vector3.Lerp( startPosition, endPosition, splinePosition.sectionT );
		}

		public override float GetTForClosestPoint (Vector3 point) 
		{
			return 0;
		}

		public override Vector3 GetTangent (float t)
		{
			CubicSplinePosition splinePosition = GetSplinePositionForT( t );
			Vector3 startTangent = calculatedPoints[splinePosition.pointIndex].tangent;
			Vector3 endTangent = calculatedPoints[splinePosition.pointIndex + 1].tangent;
			return Vector3.Lerp( startTangent, endTangent, splinePosition.sectionT );
		}

		public override Vector3 GetNormal (float t)
		{
			CubicSplinePosition splinePosition = GetSplinePositionForT( t );
			Quaternion startRotation = calculatedPoints[splinePosition.pointIndex].rotation;
			Quaternion endRotation = calculatedPoints[splinePosition.pointIndex + 1].rotation;
			normalTransform.rotation = Quaternion.Slerp( startRotation, endRotation, splinePosition.sectionT );
			return normalTransform.up;
		}
	}
}