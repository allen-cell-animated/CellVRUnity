using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Microtubule : MonoBehaviour 
	{
		public float radius = 7.5f;
		public int tubulinsPerTurn = 13;
		public Vector3[] points = new Vector3[4];
		public GameObject[] tubulinPrefabs = new GameObject[2];
		public bool renderSpline = true;
		public float[] drawTubulinRange = new float[2];
		public BezierSpline spline;

		List<Tubulin> tubulins;
		Transform tubulinParent;

		void Start () 
		{
			MakeSpline();
			AddTubulins();
		}

		void MakeSpline ()
		{
			spline = new GameObject( "spline", new System.Type[]{ typeof(BezierSpline) } ).GetComponent<BezierSpline>();
			spline.transform.position = transform.position;
			spline.transform.SetParent( transform );
			spline.MakeCurve( points, renderSpline );
		}

		void AddTubulins ()
		{
			if (tubulinPrefabs[0] == null)
			{
				return;
			}
			if (tubulinPrefabs[1] == null)
			{
				tubulinPrefabs[1] = tubulinPrefabs[0];
			}

			tubulins = new List<Tubulin>();
			tubulinParent = new GameObject( "tubulins" ).transform;
			tubulinParent.SetParent( transform );

			float length = spline.length;
			float t = 0;
			float inc = 4f / length;
			float turns = 1f / inc;
			Vector3 normal = spline.GetNormal( 0 );
			for (int i = 0; i < turns; i++)
			{
				if (t > drawTubulinRange[0] && t < drawTubulinRange[1])
				{
					Vector3 axisPosition = spline.GetPoint( t );
					Vector3 toNextAxisPosition = spline.GetPoint( t + 2f * inc ) - axisPosition; 
					int type = (i % 2 == 1) ? 0 : 1;

					for (int j = 0; j < tubulinsPerTurn; j++)
					{
						float axialOffset = (float)j / (float)tubulinsPerTurn;
						Vector3 position = axisPosition + axialOffset * toNextAxisPosition + radius * normal;
						Vector3 tangent = spline.GetTangent( t + 2f * axialOffset * inc );
						Vector3 lookDirection = Vector3.Normalize( Vector3.Cross( tangent, normal ) );
						var n = i * j + j;

						tubulins.Add( MakeTubulin( n, position, lookDirection, normal, type ) );

						normal = Quaternion.AngleAxis( 360f / (float)tubulinsPerTurn, tangent ) * normal;
					}
				}
				t += inc;
			}
		}

		Tubulin MakeTubulin (int index, Vector3 position, Vector3 lookDirection, Vector3 normal, int type)
		{
			GameObject t = Instantiate( tubulinPrefabs[type], tubulinParent ) as GameObject;
			Tubulin tubulin = t.GetComponent<Tubulin>();
			tubulin.name = "tubulin" + index;
			tubulin.transform.localPosition = position;
			tubulin.transform.LookAt( tubulin.transform.position + lookDirection, normal );

			return tubulin;
		}
	}
}