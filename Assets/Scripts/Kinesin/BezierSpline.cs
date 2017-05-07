﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class BezierSpline : MonoBehaviour 
	{
		public Vector3[] points = new Vector3[4];
		public Color lineColor;
		public int renderSegments = 15;

		float _length;
		public float length
		{
			get {
				if (_length == 0)
				{
					SetLength();
				}
				return _length;
			}
		}

		void SetLength ()
		{
			_length = 0;
			float t = 0;
			float inc = 1f / (float)renderSegments;
			for (int i = 0; i < renderSegments; i++)
			{
				_length += GetSegmentLength( t, t + inc );
				t += inc;
			}
		}

		public void MakeCurve (Vector3[] _points, bool draw)
		{
			points = _points;
			if (draw) 
			{ 
				DrawCurve(); 
			}
		}

		void DrawCurve ()
		{
			float t = 0;
			float inc = 1f / renderSegments;
			for (int i = 0; i < renderSegments; i++)
			{
				DrawLine(i, GetPoint( t ), GetPoint( t + inc ));
				t += inc;
			}
		}

		void DrawLine (int index, Vector3 start, Vector3 end)
		{
			LineRenderer lineRenderer = new GameObject( "line" + index, new System.Type[]{ typeof(LineRenderer) } ).GetComponent<LineRenderer>();
			lineRenderer.transform.position = start;
			lineRenderer.material = new Material( Shader.Find( "Particles/Alpha Blended Premultiply" ) );
			lineRenderer.startColor = lineRenderer.endColor = lineColor;
			lineRenderer.startWidth = lineRenderer.endWidth = 0.1f;
			lineRenderer.SetPosition( 0, start );
			lineRenderer.SetPosition( 1, end );
			lineRenderer.transform.SetParent( transform );
		}

		float GetSegmentLength (float startT, float endT)
		{
			return Vector3.Distance( GetPoint( startT ), GetPoint( endT ) );
		}

		public Vector3 GetPoint (float t)
		{
			return new Vector3(
				CubicBezier( t, points[0].x, points[1].x, points[2].x, points[3].x ),
				CubicBezier( t, points[0].y, points[1].y, points[2].y, points[3].y ),
				CubicBezier( t, points[0].z, points[1].z, points[2].z, points[3].z )
			);
		}

		float CubicBezier (float t, float point0, float point1, float point2, float point3)
		{
			return CubicBezier_Point0( t, point0 ) 
				+ CubicBezier_Point1( t, point1 ) 
				+ CubicBezier_Point2( t, point2 ) 
				+ CubicBezier_Point3( t, point3 );
		}

		float CubicBezier_Point0 (float t, float point0)
		{
			return (1f - t) * (1f - t) * (1f - t) * point0;
		}

		float CubicBezier_Point1 (float t, float point1)
		{
			return 3f * (1f - t) * (1f - t) * t * point1;
		}

		float CubicBezier_Point2 (float t, float point2)
		{
			return 3f * (1f - t) * t * t * point2;
		}

		float CubicBezier_Point3 (float t, float point3)
		{
			return t * t * t * point3;
		}

		public float GetTForClosestPoint (Vector3 point) 
		{
			return FindClosest( point, 0f, 1f, 10 );
		}

		float FindClosest (Vector3 point, float t1, float t2, int iterations)
		{
			float[] distance = new float[]{ Vector3.Distance( point, GetPoint( t1 ) ), Vector3.Distance( point, GetPoint( t2 ) ) };
			float middle = (t1 + t2) / 2f;
			if (distance[0] > distance[1])
			{
				if (iterations > 0)
				{
					return FindClosest( point, middle, t2, iterations - 1 );
				}
				return t2;
			}
			else
			{
				if (iterations > 0)
				{
					return FindClosest( point, t1, middle, iterations - 1 ); 
				}
				return t1;
			}
		}

		public Vector3 GetNormal (float t)
		{
			float inc = 0.001f;
			if (t >= 1f)
			{
				inc *= -1f;
			}

			Vector3 tangent = GetTangent( t );

			Vector3 tangentInc = GetTangent( t + inc );
			tangentInc -= GetPoint( t ) - GetPoint( t + inc );

			return Vector3.Normalize( Vector3.Cross( tangent, tangentInc ) );
		}

		public Vector3 GetTangent (float t)
		{
			return Vector3.Normalize( new Vector3(
				CubicBezierTangent( t, points[0].x, points[1].x, points[2].x, points[3].x ),
				CubicBezierTangent( t, points[0].y, points[1].y, points[2].y, points[3].y ),
				CubicBezierTangent( t, points[0].z, points[1].z, points[2].z, points[3].z )
			));
		}

		float CubicBezierTangent (float t, float point0, float point1, float point2, float point3)
		{
			return CubicBezierTangent_Point0_to_Point1( t, point0, point1 ) 
				+ CubicBezierTangent_Point1_to_Point2( t, point1, point2 ) 
				+ CubicBezierTangent_Point2_to_Point3( t, point2, point3 );
		}

		float CubicBezierTangent_Point0_to_Point1 (float t, float point0, float point1)
		{
			return 3f * (1f - t) * (1f - t) * (point1 - point0);
		}

		float CubicBezierTangent_Point1_to_Point2 (float t, float point1, float point2)
		{
			return 6f * (1f - t) * t * (point2 - point1);
		}

		float CubicBezierTangent_Point2_to_Point3 (float t, float point2, float point3)
		{
			return 3f * t * t * (point3 - point2);
		}
	}
}
