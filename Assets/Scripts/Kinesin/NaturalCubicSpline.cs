using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
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
		public float[] arcLengths;

		// ---------------------------------------------- Length

		public override float GetLength ()
		{
			float l = 0;
			for (int i = 1; i < n; i++)
			{
				l += arcLengths[i];
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
			arcLengths = new float[n];
			int k = 0;
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
						if (s == 0)
						{
							arcLengths[section] = arcLength;
							arcLength = 0;
						}
					}
					k++;
				}
			}
			arcLengths[n - 1] = arcLength;
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

		CubicSplinePosition GetSplinePositionForT (float t)
		{
			float tLength = t * length;
			float currentLength = 0;
			for (int i = 1; i < arcLengths.Length; i++)
			{
				if (tLength < currentLength + arcLengths[i])
				{
					return new CubicSplinePosition( i - 1, (tLength - currentLength) / arcLengths[i] );
				}
				currentLength += arcLengths[i];
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

		public override Vector3 GetNormal (float t)
		{
			float inc = 0.001f;
			if (t >= 1)
			{
				inc *= -1;
			}
			Vector3 tangent = GetTangent( t );
			Vector3 incTangent = GetTangent( t + inc ) - (GetPoint( t ) - GetPoint( t + inc ));
			return Vector3.Cross( tangent, incTangent );
		}

		public override Vector3 GetTangent (float t)
		{
			CubicSplinePosition position = GetSplinePositionForT( t );
			return CalculateTangent( position.pointIndex, position.sectionT );
		}
	}
}