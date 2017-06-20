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
		public Vector3[] segmentPositions;
		public Transform[] points;

		protected bool pointsAreSet
		{
			get {
				return points != null && points.Length > 0;
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
			if (pointsAreSet && pointsChanged)
			{
				UpdateCurve();
				if (drawCurve)
				{
					ClearExtraLines();
					Draw();
				}
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

		Transform _normalTransform;
		Transform normalTransform
		{
			get
			{
				if (_normalTransform == null)
				{
					_normalTransform = new GameObject( "Normal" ).transform;
					_normalTransform.SetParent( transform );
					_normalTransform.localPosition = Vector3.zero;
				}
				return _normalTransform;
			}
		}

		public Vector3 GetNormal (float t)
		{
			normalTransform.LookAt( transform.position + GetTangent( t ) );
			return normalTransform.up;
		}

		public abstract Vector3 GetTangent (float t);
	}
}
