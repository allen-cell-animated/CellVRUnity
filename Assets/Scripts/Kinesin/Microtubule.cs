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

		Transform tubulinParent;
		List<Tubulin> _tubulins;
		List<Tubulin> tubulins
		{
			get
			{
				if (_tubulins == null)
				{
					_tubulins = new List<Tubulin>();
					tubulinParent = new GameObject( "tubulins" ).transform;
					tubulinParent.SetParent( transform );
					tubulinParent.localPosition = Vector3.zero;
				}
				return _tubulins;
			}
		}

		bool canMakeTubulins
		{
			get 
			{
				bool make = true;
				if ((tubulinPrefabs[0] == null && tubulinPrefabs[1] == null) || spline == null)
				{
					make = false;
				}
				else if (tubulinPrefabs[0] == null)
				{
					tubulinPrefabs[0] = tubulinPrefabs[1];
				}
				else if (tubulinPrefabs[1] == null)
				{
					tubulinPrefabs[1] = tubulinPrefabs[0];
				}
				return make;
			}
		}

		void Update ()
		{
			if (spline != null && spline.UpdateSpline())
			{
				PlaceTubulins();
			}
		}

		void PlaceTubulins ()
		{
			Debug.Log("Place " + Mathf.Round( Time.time ));
			float t = 0, turns = spline.length / 4f, inc = 1f / turns, rotationPerTubulin = 360f / (float)tubulinsPerTurn, normalRotation = 0;
			bool reachedEndOfSpline = false;
			int k = 0;
			for (int i = 0; i < turns; i++)
			{
				Vector3 turnPosition = spline.GetPosition( t );
				Vector3 nextTurnPosition = spline.GetPosition( t + 2 * inc );

				for (int j = 0; j < tubulinsPerTurn; j++)
				{
					if (k >= tubulins.Count)
					{
						tubulins.Add( MakeTubulin( k ) );
					}

					if (tubulins[k] != null)
					{
						if (!reachedEndOfSpline)
						{
							float axialOffset = 1.5f * j / (float)tubulinsPerTurn;
							float turnT = t + 2f * axialOffset * inc;
							if (turnT > 1f)
							{
								reachedEndOfSpline = true;
							}
							Vector3 tangent = spline.GetTangent( turnT );
							Vector3 normal = Quaternion.AngleAxis( normalRotation, tangent ) * spline.GetNormal( turnT );
							Vector3 position = turnPosition + axialOffset * (nextTurnPosition - turnPosition) + radius * normal;
							Vector3 lookDirection = Vector3.Normalize( Vector3.Cross( tangent, normal ) );

							tubulins[k].gameObject.SetActive( true );
							tubulins[k].Place( position, lookDirection, normal );

							normalRotation += rotationPerTubulin;
						}
						else
						{
							tubulins[k].gameObject.SetActive( false );
						}
					}
					k++;
				}
				t += inc;
			}
			for (int i = k; i < tubulins.Count; i++)
			{
				tubulins[i].gameObject.SetActive( false );
			}
		}

		Tubulin MakeTubulin (int index)
		{
			if (canMakeTubulins)
			{
				int type = (index % (2f * tubulinsPerTurn) < tubulinsPerTurn ? 0 : 1);
				GameObject tubulin = Instantiate( tubulinPrefabs[type], tubulinParent ) as GameObject;
				tubulin.name = tubulinPrefabs[type].name + "_" + index;

				return tubulin.GetComponent<Tubulin>();
			}
			return null;
		}
	}
}