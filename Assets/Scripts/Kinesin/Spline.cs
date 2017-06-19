using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public abstract class Spline : MonoBehaviour 
	{
		public int resolution = 15;
		public float updateTolerance = 0.1f;
		public bool drawCurve;
		public Color lineColor = new Color( 1f, 0, 1f );
		public Transform[] points;
		public Vector3[] segmentPositions;

		protected bool pointsAreSet
		{
			get {
				return points != null && points.Length > 1;
			}
		}

		void Start ()
		{
			if (pointsAreSet)
			{
				lastPointPositions = new Vector3[points.Length];
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
					_length = GetLength();
					lastLengthTime = Time.time;
				}
				return _length;
			}
		}

		public abstract float GetLength ();

		// ---------------------------------------------- Point Positions

		public Vector3[] lastPointPositions;

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

		protected List<LineRenderer> lines = new List<LineRenderer>();

		void Update ()
		{
			if (drawCurve && pointsAreSet && pointsChanged)
			{
				UpdateCurve();
				ClearExtraLines();
				Draw();
			}
		}

		protected abstract void UpdateCurve ();

		protected abstract void Draw ();

		protected void ClearExtraLines ()
		{
			if (lines.Count > segmentPositions.Length) 
			{ 
				int currentN = lines.Count;
				for (int i = segmentPositions.Length; i < currentN; i++)
				{
					Destroy( lines[i].gameObject );
				}
				lines.RemoveRange( segmentPositions.Length, lines.Count - segmentPositions.Length );
			}
		}

		protected void DrawSegment (int index, Vector3 start, Vector3 end)
		{
			if (index >= lines.Count)
			{
				lines.Add( null );
			}
			if (lines[index] == null)
			{
				lines[index] = new GameObject( "line" + index, new System.Type[]{ typeof(LineRenderer) } ).GetComponent<LineRenderer>();
				lines[index].material = new Material( Shader.Find( "Particles/Alpha Blended Premultiply" ) );
				lines[index].startColor = lines[index].endColor = lineColor;
				lines[index].startWidth = lines[index].endWidth = 1f;
				lines[index].transform.SetParent( transform );
			}
			lines[index].transform.position = start;
			lines[index].SetPosition( 0, start );
			lines[index].SetPosition( 1, end );
		}

		// ---------------------------------------------- Calculation

		public abstract Vector3 GetPoint (float t);

		public abstract float GetTForClosestPoint (Vector3 point);

		public abstract Vector3 GetNormal (float t);

		public abstract Vector3 GetTangent (float t);

		public abstract Vector3[] GetIncrementalPoints (float distanceIncrement);
	}
}
