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
		public BezierSpline spline;

		List<Tubulin> tubulins;
		Transform tubulinParent;
		public float turns;

		void Start () 
		{
			MakeSpline();
			MakeTubulins();
		}

		void MakeSpline ()
		{
			if (spline == null)
			{
				spline = new GameObject( "spline", new System.Type[]{ typeof(BezierSpline) } ).GetComponent<BezierSpline>();
				spline.transform.position = transform.position;
				spline.transform.SetParent( transform );
				spline.MakeCurve( points, renderSpline );
			}
		}

		void MakeTubulins ()
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

			turns = spline.length / 4f;
			for (int i = 0; i < turns * tubulinsPerTurn; i++)
			{
				int type = (i % 2 == 1) ? 0 : 1;
				tubulins.Add( MakeTubulin( i, type ) );
			}

			PlaceTubulins();
		}

		Tubulin MakeTubulin (int index, int type)
		{
			GameObject t = Instantiate( tubulinPrefabs[type], tubulinParent ) as GameObject;
			Tubulin tubulin = t.GetComponent<Tubulin>();
			tubulin.name = "tubulin" + index;

			return tubulin;
		}

		void PlaceTubulins ()
		{
			float t = 0;
			float inc = (4f * turns / spline.length) * 4f / spline.length;
			Vector3 normal = spline.GetNormal( 0 );
			for (int i = 0; i < turns; i++)
			{
				Vector3 axisPosition = spline.GetPoint( t );
				Vector3 nextAxisPosition = spline.GetPoint( t + 2 * inc );

				for (int j = 0; j < tubulinsPerTurn; j++)
				{
					int index = i * tubulinsPerTurn + j;
					if (index >= tubulins.Count)
					{
						return;
					}

					float axialOffset = (float)j / (float)tubulinsPerTurn;
					Vector3 position = axisPosition + axialOffset * (nextAxisPosition - axisPosition) + radius * normal;
					Vector3 tangent = spline.GetTangent( t + 2f * axialOffset * inc );
					Vector3 lookDirection = Vector3.Normalize( Vector3.Cross( tangent, normal ) );

					tubulins[index].Place( position, lookDirection, normal );

					normal = Quaternion.AngleAxis( 360f / (float)tubulinsPerTurn, tangent ) * normal;
				}
				t += inc;
			}
		}

		void Update ()
		{
			if (spline.pointsChanged)
			{
				PlaceTubulins();
			}
		}
	}
}