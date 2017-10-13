using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Splines;

namespace AICS.MacroMolecules
{
	public class MicrotubuleComponentSpawner : ComponentSpawner
	{
		public int tubulinsPerTurn = 13;

		ComponentMolecule[] _tubulinPrefabs;
		ComponentMolecule[] tubulinPrefabs
		{
			get
			{
				if (_tubulinPrefabs == null)
				{
					_tubulinPrefabs = new ComponentMolecule[2];
					_tubulinPrefabs[0] = componentPrefabs.Find( m => m.type == MoleculeType.TubulinA );
					_tubulinPrefabs[1] = componentPrefabs.Find( m => m.type == MoleculeType.TubulinB );
				}
				return _tubulinPrefabs;
			}
		}

		Spline _spline;
		Spline spline
		{
			get
			{
				if (_spline == null)
				{
					_spline = GetComponentInChildren<Spline>();
				}
				return _spline;
			}
		}

		protected override void SpawnAll ()
		{
			float middleT = PlaceHalfOfTubulins( -1, 0.5f );
			PlaceHalfOfTubulins( 1, middleT );
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
			List<ComponentMolecule> tubulins = new List<ComponentMolecule>();
			for (int i = 0; i < turns; i++)
			{
				for (int j = 0; j < tubulinsPerTurn; j++)
				{
					if (!reachedEndOfSpline)
					{
						if (k >= tubulins.Count)
						{
							int type = direction < 0 ? (k % (2 * tubulinsPerTurn) < tubulinsPerTurn ? 0 : 1) : (k % (2 * tubulinsPerTurn) < tubulinsPerTurn ? 1 : 0);
							tubulins.Add( SpawnComponent( tubulinPrefabs[type] ) );
						}
						if (tubulins[k] != null)
						{
							float turnT = t + 3f * j / (float)tubulinsPerTurn * inc;
							Vector3 tangent = spline.GetTangent( turnT );
							Vector3 normal = Quaternion.AngleAxis( normalRotation, tangent ) * spline.GetNormal( turnT );
							Vector3 position = spline.GetPosition( turnT ) + assemblyMolecule.radius * normal;
							Vector3 lookDirection = Vector3.Normalize( Vector3.Cross( tangent, normal ) );

							tubulins[k].gameObject.SetActive( true );
							PlaceComponent( tubulins[k], position, lookDirection, normal );

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
	}
}