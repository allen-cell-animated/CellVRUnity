using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Microtubule
{
	public class Microtubule : MonoBehaviour 
	{
		public float radius = 9f;
		public int tubulinsPerTurn = 13;
		public GameObject[] tubulinPrefabs = new GameObject[2];
		public Spline spline;

		bool placedTubulins;

		Transform[] _tubulinParents;
		Transform GetTubulinParent (int index)
		{
			if (_tubulinParents == null)
			{
				_tubulinParents = new Transform[2];
				for (int i = 0; i < 2; i++)
				{
					_tubulinParents[i] = new GameObject( "tubulins" + i ).transform;
					_tubulinParents[i].SetParent( transform );
					_tubulinParents[i].localPosition = Vector3.zero;
				}
			}
			return _tubulinParents[index];
		}

		List<Tubulin>[] _tubulins;
		List<Tubulin> GetTubulins (int index)
		{
			if (_tubulins == null)
			{
				_tubulins = new List<Tubulin>[2];
				for (int i = 0; i < 2; i++)
				{
					_tubulins[i] = new List<Tubulin>();
				}
			}
			return _tubulins[index];
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
			if (spline != null && (!placedTubulins || spline.UpdateSpline()))
			{
				PlaceTubulins();
			}
		}

		void PlaceTubulins ()
		{
			float middleT = PlaceHalfOfTubulins( -1, 0.5f );
			PlaceHalfOfTubulins( 1, middleT );
			placedTubulins = true;
		}

		float PlaceHalfOfTubulins (int direction, float startT)
		{
			int k = 0;
			float middleT = 0, t = startT;
			float turns = spline.length / 4f;
			float inc = direction * 1f / turns;
			float rotationPerTubulin = direction * 360f / (float)tubulinsPerTurn;
			float normalRotation = (direction < 0) ? 0 : rotationPerTubulin;
			bool reachedEndOfSpline = false;
			List<Tubulin> tubulins = GetTubulins( (direction > 0) ? 1 : 0 );
			for (int i = 0; i < turns; i++)
			{
				for (int j = 0; j < tubulinsPerTurn; j++)
				{
					if (!reachedEndOfSpline)
					{
						if (k >= tubulins.Count)
						{
							tubulins.Add( MakeTubulin( k, direction ) );
						}
						if (tubulins[k] != null)
						{
							float turnT = t + 3f * j / (float)tubulinsPerTurn * inc;
							Vector3 tangent = spline.GetTangent( turnT );
							Vector3 normal = Quaternion.AngleAxis( normalRotation, tangent ) * spline.GetNormal( turnT );
							Vector3 position = spline.GetPosition( turnT ) + radius * normal;
							Vector3 lookDirection = Vector3.Normalize( Vector3.Cross( tangent, normal ) );

							tubulins[k].gameObject.SetActive( true );
							tubulins[k].Place( position, lookDirection, normal );

							normalRotation += rotationPerTubulin;
							if (turnT >= 1f || turnT <= 0)
							{
								reachedEndOfSpline = true;
							}
							if (k == 2 * tubulinsPerTurn)
							{
								middleT = turnT;
							}
						}
					}
					else if (k < tubulins.Count)
					{
						tubulins[k].gameObject.SetActive( false );
					}
					k++;
				}
				t += inc;
			}
			for (int i = k; i < tubulins.Count; i++)
			{
				tubulins[i].gameObject.SetActive( false );
			}
			return middleT;
		}

		Tubulin MakeTubulin (int index, int direction)
		{
			if (canMakeTubulins)
			{
				int type = (index % (2 * tubulinsPerTurn) < tubulinsPerTurn) ? 0 : 1;
				GameObject tubulin = Instantiate( tubulinPrefabs[type], GetTubulinParent( (direction < 0) ? 0 : 1 ) ) as GameObject;
				tubulin.name = tubulinPrefabs[type].name + "_" + index;

				return tubulin.GetComponent<Tubulin>();
			}
			return null;
		}
	}
}