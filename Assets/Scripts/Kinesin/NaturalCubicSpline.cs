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
		public Quaternion normal;
		public float arcLength;

		public CubicSplinePoint (Vector3 _position, Vector3 _tangent, Quaternion _normal, float _arcLength)
		{
			position = _position;
			tangent = _tangent;
			normal = _normal;
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
			for (int i = 1; i < n; i++)
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
			calculatedPoints = new CubicSplinePoint[n];
			int k = 0;
			Vector3 tangent;
			normalTransform.rotation = Quaternion.identity;
			float arcLength = 0;
			for (int section = 0; section < n - 1; section++)
			{
				int segments = (section < n - 2 ? divisions + 1 : divisions + 2);
				for (int s = 0; s < segments; s++)
				{
					segmentPositions[k] = CalculatePosition( section, s / (divisions + 1f) );
					if (k > 0)
					{
						arcLength += Vector3.Distance( segmentPositions[k], segmentPositions[k - 1] );
					}
					if (s == 0)
					{
						tangent = CalculateTangent( section, 0 );
						calculatedPoints[section] = new CubicSplinePoint( points[section].position, tangent, CalculateNormal( section, tangent ), arcLength );
						arcLength = 0;
					}
					k++;
				}
			}
			tangent = CalculateTangent( n - 2, 1f );
			calculatedPoints[n - 1] = new CubicSplinePoint( points[n - 1].position, tangent, CalculateNormal( n - 1, tangent ), arcLength );
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

		Quaternion CalculateNormal (int pointIndex, Vector3 tangent)
		{
			Vector3 t0;
			if (pointIndex == 0)
			{
				t0 = Vector3.forward;
				tangent = CalculateTangent( pointIndex, 0 );
			}
			else
			{
				t0 = CalculateTangent( pointIndex - 1, 0 );
				tangent = CalculateTangent( pointIndex - 1, 1f );
			}
			Quaternion rotation = Quaternion.AngleAxis( 180f * Mathf.Acos( Vector3.Dot( normalTransform.forward, tangent ) ) / Mathf.PI, 
				Vector3.Normalize( Vector3.Cross( normalTransform.forward, tangent ) ) );
			normalTransform.rotation *= rotation;
			Transform test = new GameObject( "normal" + pointIndex ).transform;
			test.position = points[pointIndex].position;
			test.rotation = normalTransform.rotation;
			Transform test1 = new GameObject( "tangent" + pointIndex ).transform;
			test1.position = points[pointIndex].position;
			test1.LookAt( test1.position + tangent );
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
			return new CubicSplinePosition( n - 2, 1f );
		}

		public override Vector3 GetPoint (float t)
		{
			CubicSplinePosition position = GetSplinePositionForT( t );
			return CalculatePosition( position.pointIndex, position.sectionT );
		}

		public override float GetTForClosestPoint (Vector3 point) 
		{
			return 0;
		}

		public override Vector3 GetTangent (float t)
		{
			CubicSplinePosition position = GetSplinePositionForT( t );
			return CalculateTangent( position.pointIndex, position.sectionT );

//			CubicSplinePosition position = GetSplinePositionForT( t );
//			Vector3 startTangent = calculatedPoints[position.pointIndex].tangent;
//			Vector3 endTangent = calculatedPoints[position.pointIndex + 1].tangent;
//			return Vector3.Lerp( startTangent, endTangent, position.sectionT );
		}

		public override Vector3 GetNormal (float t)
		{
			CubicSplinePosition position = GetSplinePositionForT( t );
			Quaternion startNormal = calculatedPoints[position.pointIndex].normal;
			Quaternion endNormal = calculatedPoints[position.pointIndex + 1].normal;
			normalTransform.rotation = Quaternion.Slerp( startNormal, endNormal, position.sectionT );
			return normalTransform.up;
		}
	}
}