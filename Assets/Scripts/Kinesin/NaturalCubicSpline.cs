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

		Matrix4x4 coefficientMatrix
		{
			get {
				Matrix4x4 a = new Matrix4x4();
				a.SetRow( 0, new Vector4( 1f, 1f, 0f, 0f ) );
				a.SetRow( 1, new Vector4( 1f, 4f, 1f, 0f ) );
				a.SetRow( 2, new Vector4( 0f, 1f, 4f, 1f ) );
				a.SetRow( 3, new Vector4( 0f, 0f, 1f, 1f ) );
				return a;
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
			int divisions = Mathf.FloorToInt( (float) renderSegments / (float) points.Length );
			segmentPositions = new Vector3[divisions * (points.Length - 1) + points.Length];
			tangents = new Vector3[segmentPositions.Length];
			for (int axis = 0; axis < 3; axis++)
			{
				Vector4 b = Vector4.zero;
				for (int i = 1; i < points.Length - 1; i++)
				{
					b[i] = points[i + 1].position[axis] - 2f * points[i].position[axis] + points[i - 1].position[axis];
				}
				Vector4 z = coefficientMatrix.inverse * b;

				int k = 0;
				for (int i = 0; i < points.Length - 1; i++)
				{
					int segments = (i < points.Length - 2 ? divisions + 1 : divisions + 2);
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