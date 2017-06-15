using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public abstract class Spline : MonoBehaviour 
	{
		public Transform[] points;
		public Color lineColor = new Color( 1f, 0, 1f );
		public int renderSegments = 15;
		public float updateTolerance = 0.1f;
		public bool drawCurve;

		void Start ()
		{
			if (points != null && points.Length > 1)
			{
				CachePointStartPositions();
				if (drawCurve) 
				{ 
					Draw(); 
				}
			}
		}

		// ---------------------------------------------- Length

		float lastLengthTime = -1f;
		float _length;

		public float length
		{
			get {
				if (Time.time - lastLengthTime > 0.1f)
				{
					_length = GetLength( 0, 1f, renderSegments );
					lastLengthTime = Time.time;
				}
				return _length;
			}
		}

		public float GetLength (float startT, float endT, int resolution)
		{
			float l = 0;
			float t = startT;
			float inc = (endT - startT) / resolution;
			for (int i = 0; i < resolution; i++)
			{
				l += GetSegmentLength( t, t + inc );
				t += inc;
			}
			return l;
		}

		float GetSegmentLength (float startT, float endT)
		{
			return Vector3.Distance( GetPoint( startT ), GetPoint( endT ) );
		}

		// ---------------------------------------------- Point Positions

		Vector3[] lastPointPositions;

		void CachePointStartPositions ()
		{
			lastPointPositions = new Vector3[points.Length];
			for (int i = 0; i < points.Length; i++)
			{
				lastPointPositions[i] = points[i].position;
			}
		}

		public bool pointsChanged
		{
			get {
				bool changed = false;
				for (int i = 0; i < points.Length; i++)
				{
					if (Vector3.Distance( points[i].position, lastPointPositions[i] ) > updateTolerance)
					{
						changed = true;
						lastPointPositions[i] = points[i].position;
					}
				}
				return changed;
			}
		}

		// ---------------------------------------------- Drawing

		void Draw ()
		{
			float t = 0;
			float inc = 1f / renderSegments;
			for (int i = 0; i < renderSegments; i++)
			{
				DrawSegment(i, transform.TransformPoint( GetPoint( t ) ), transform.TransformPoint( GetPoint( t + inc ) ) );
				t += inc;
			}
		}

		void DrawSegment (int index, Vector3 start, Vector3 end)
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

		// ---------------------------------------------- Calculation

		public abstract Vector3 GetPoint (float t);

		public abstract float GetTForClosestPoint (Vector3 point);

		public abstract Vector3 GetNormal (float t);

		public abstract Vector3 GetTangent (float t);
	}
}
