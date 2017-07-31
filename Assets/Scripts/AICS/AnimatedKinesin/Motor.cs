using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Microtubule;

namespace AICS.AnimatedKinesin
{
	public class Motor : Molecule 
	{
		Tubulin tubulin;

		void FixedUpdate () 
		{
			Rotate();
			Move();
		}

		protected override bool WillCollide (Vector3 moveStep)
		{
			RaycastHit[] hits = body.SweepTestAll( moveStep.normalized, moveStep.magnitude, UnityEngine.QueryTriggerInteraction.Collide );
			if (hits.Length > 0)
			{
				CheckHitsForTubulin( hits );
				return true;
			}
			return false;
		}

		void CheckHitsForTubulin (RaycastHit[] hits)
		{
			Tubulin _tubulin;
			List<Tubulin> tubulins = new List<Tubulin>();
			foreach (RaycastHit hit in hits)
			{
				_tubulin = hit.collider.GetComponent<Tubulin>();
				if (_tubulin != null)
				{
					tubulins.Add( _tubulin );
				}
			}

			if (tubulins.Count > 0)
			{
				BindTubulin( FindClosestTubulin( tubulins ) );
			}
		}

		Tubulin FindClosestTubulin (List<Tubulin> tubulins)
		{
			if (tubulins.Count == 1)
			{
				return tubulins[0];
			}

			float d, minDistance = Mathf.Infinity;
			Tubulin closestTubulin = tubulins[0];
			foreach (Tubulin _tubulin in tubulins)
			{
				d = Vector3.Distance( _tubulin.transform.position, transform.position );
				if (d < minDistance)
				{
					minDistance = d;
					closestTubulin = _tubulin;
				}
			}
			return closestTubulin;
		}

		void BindTubulin (Tubulin tubulin)
		{
			Debug.Log( name + " bind " + tubulin.name );
		}
	}
}