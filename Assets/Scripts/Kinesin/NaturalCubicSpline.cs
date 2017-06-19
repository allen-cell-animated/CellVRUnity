using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class NaturalCubicSpline : Spline 
	{
		protected override void Draw ()
		{
			CheckCalculateCurve();
			for (int i = 0; i < segmentPositions.Length - 1; i++)
			{
				DrawSegment( i, segmentPositions[i], segmentPositions[i + 1] );
			}
		}

		public Vector3[] segmentPositions;
		public Vector3[] tangents;

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

		bool needToCalculateCurve
		{
			get {
				return pointsAreSet && (segmentPositions == null || segmentPositions.Length < 2 || pointsChanged);
			}
		}

		bool CheckCalculateCurve ()
		{
			if (needToCalculateCurve)
			{
				CalculateCurve();
				return true;
			}
			return false;
		}

		void CalculateCurve ()
		{
			int divisions = Mathf.FloorToInt( (float) renderSegments / (float) n );
			segmentPositions = new Vector3[divisions * (n - 1) + n];
			tangents = new Vector3[segmentPositions.Length];
			for (int axis = 0; axis < 3; axis++)
			{
				float[] b = new float[n];
				for (int i = 1; i < n - 1; i++)
				{
					b[i] = points[i + 1].position[axis] - 2f * points[i].position[axis] + points[i - 1].position[axis];
				}
				float[] z = coefficientMatrix.inverse * b;

				int k = 0;
				for (int i = 0; i < n - 1; i++)
				{
					int segments = (i < n - 2 ? divisions + 1 : divisions + 2);
					for (int s = 0; s < segments; s++)
					{
						float t = s / (divisions + 1f);
						float ct = points[i + 1].position[axis] - z[i + 1];
						float c1t = points[i].position[axis] - z[i];
						segmentPositions[k][axis] = z[i + 1] * Mathf.Pow( t, 3f ) + z[i] * Mathf.Pow( 1f - t, 3f ) + ct * t + c1t * (1f - t);
						tangents[k][axis] = 3f * z[i + 1] * Mathf.Pow( t, 2f ) - 3f * z[i] * Mathf.Pow( 1f - t, 2f ) + ct - c1t;
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

		}

		public override Vector3 GetPoint (float t)
		{
			CheckCalculateCurve();
			return Vector3.zero;
		}

		public override float GetTForClosestPoint (Vector3 point) 
		{
			CheckCalculateCurve();
			return 0;
		}

		public override Vector3 GetNormal (float t)
		{
			CheckCalculateCurve();
			return Vector3.zero;
		}

		public override Vector3 GetTangent (float t)
		{
			CheckCalculateCurve();
			return Vector3.zero;
		}

		void Update ()
		{
			if (CheckCalculateCurve())
			{
				Draw();
			}
		}
	}
}