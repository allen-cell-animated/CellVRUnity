using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class NaturalCubicSpline : Spline 
	{
		public float segmentLength;
		public Vector3[] tangents;

		float[][] linearSystemSolution;

		// ---------------------------------------------- Length

		public override float GetLength ()
		{
			float l = 0;
			for (int i = 0; i < segmentPositions.Length - 1; i++)
			{
				l += SegmentLength( i );
			}
			return l;
		}

		float SegmentLength (int index)
		{
			return Vector3.Distance( segmentPositions[index], segmentPositions[index + 1] );
		}

		// ---------------------------------------------- Drawing

		protected override void UpdateCurve ()
		{
			CalculateCurve();
			EquispaceSegments();
		}

		protected override void Draw ()
		{
			for (int i = 0; i < segmentPositions.Length - 1; i++)
			{
				DrawSegment( i, segmentPositions[i], segmentPositions[i + 1] );
			}
		}

		// ---------------------------------------------- Calculation

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
			int divisions = Mathf.FloorToInt( (float) resolution / (float) n );
			segmentPositions = new Vector3[divisions * (n - 1) + n];
			tangents = new Vector3[segmentPositions.Length];
			linearSystemSolution = new float[3][];
			for (int axis = 0; axis < 3; axis++)
			{
				float[] b = new float[n];
				for (int i = 1; i < n - 1; i++)
				{
					b[i] = points[i + 1].position[axis] - 2f * points[i].position[axis] + points[i - 1].position[axis];
				}
				linearSystemSolution[axis] = coefficientMatrix.inverse * b;

				int k = 0;
				for (int i = 0; i < n - 1; i++)
				{
					int segments = (i < n - 2 ? divisions + 1 : divisions + 2);
					for (int s = 0; s < segments; s++)
					{
						float t = s / (divisions + 1f);
						float ct = points[i + 1].position[axis] - linearSystemSolution[axis][i + 1];
						float c1t = points[i].position[axis] - linearSystemSolution[axis][i];
						segmentPositions[k][axis] = linearSystemSolution[axis][i + 1] * Mathf.Pow( t, 3f ) + linearSystemSolution[axis][i] * Mathf.Pow( 1f - t, 3f ) 
							+ ct * t + c1t * (1f - t);
						tangents[k][axis] = 3f * linearSystemSolution[axis][i + 1] * Mathf.Pow( t, 2f ) - 3f * linearSystemSolution[axis][i] * Mathf.Pow( 1f - t, 2f ) 
							+ ct - c1t;
						k++;
					}
				}
			}
			for (int i = 0; i < tangents.Length; i++)
			{
				tangents[i] = Vector3.Normalize( tangents[i] );
			}
		}

		void EquispaceSegments ()
		{
			EquispaceSegments( segmentLength );
		}

		void EquispaceSegments (float spacing)
		{
			segmentLength = spacing;
			List<Vector3> newSegmentPositions = new List<Vector3>();
			float leftoverDistance = segmentLength;
			for (int i = 0; i < segmentPositions.Length - 1; i++)
			{
				float l = SegmentLength( i );
				float tInc = segmentLength / l;
				float t = (segmentLength - leftoverDistance) / l;
				while (t < 1f)
				{
					newSegmentPositions.Add( GetPointOnSegment( i, t ) );
					t += tInc;
				}
				leftoverDistance = (1 - (t - tInc)) * l;
			}
			segmentPositions = newSegmentPositions.ToArray();
		}

		Vector3 GetPointOnSegment (int index, float t)
		{
			return Vector3.Lerp( segmentPositions[index], segmentPositions[index + 1], t );
		}

		public override Vector3 GetPoint (float t)
		{
			UpdateCurve();
			return Vector3.zero;
		}

		public override float GetTForClosestPoint (Vector3 point) 
		{
			UpdateCurve();
			return 0;
		}

		public override Vector3 GetNormal (float t)
		{
			UpdateCurve();
			return Vector3.zero;
		}

		public override Vector3 GetTangent (float t)
		{
			UpdateCurve();
			return Vector3.zero;
		}

		public override Vector3[] GetIncrementalPoints (float spacing)
		{
			if (segmentLength < spacing - 1f || segmentLength > spacing + 1f)
			{
				EquispaceSegments( spacing );
			}
			return segmentPositions;
		}
	}
}