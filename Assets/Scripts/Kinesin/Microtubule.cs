using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Microtubule : MonoBehaviour 
	{
		public float radius = 9f;
		public int tubulinsPerTurn = 13;
		public GameObject[] tubulinPrefabs = new GameObject[2];
		public Spline spline;

		List<Tubulin> tubulins;
		Transform tubulinParent;
		float turns;

		void Start () 
		{
			MakeTubulins();
		}

		void MakeTubulins ()
		{
			if (tubulinPrefabs[0] == null || spline == null)
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
			int type = 0;
			for (int i = 0; i < turns * tubulinsPerTurn; i++)
			{
				if (i % tubulinsPerTurn == 0)
				{
					type++;
					if (type > 1)
					{
						type = 0;
					}
				}
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
			float rotationPerTubulin = 360f / (float)tubulinsPerTurn;
			float normalRotation = 0;
			bool place = true;
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

					if (place)
					{
						float axialOffset = 1.5f * (float)j / (float)tubulinsPerTurn;
						float turnT = t + 2f * axialOffset * inc;
						if (turnT > 1)
						{
							place = false;
						}
						Vector3 tangent = spline.GetTangent( turnT );
						Vector3 normal = Quaternion.AngleAxis( normalRotation, tangent ) * spline.GetNormal( turnT );
						Vector3 position = axisPosition + axialOffset * (nextAxisPosition - axisPosition) + radius * normal;
						Vector3 lookDirection = Vector3.Normalize( Vector3.Cross( tangent, normal ) );

						tubulins[index].Place( position, lookDirection, normal );

						normalRotation += rotationPerTubulin;
					}
					else
					{
						tubulins[index].gameObject.SetActive( false );
					}
				}
				t += inc;
			}
		}

		void Update ()
		{
			if (spline != null && spline.pointsChanged)
			{
				PlaceTubulins();
			}
		}
	}
}