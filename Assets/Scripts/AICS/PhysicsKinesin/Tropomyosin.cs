using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.PhysicsKinesin
{
	public class Tropomyosin : MonoBehaviour 
	{
		Organelle _organelle;
		public Organelle organelle
		{
			get {
				if (_organelle == null)
				{
					_organelle = GetComponentInParent<Organelle>();
				}
				return _organelle;
			}
		}

		Hips _hips;
		public Hips hips
		{
			get {
				if (_hips == null)
				{
					_hips = GameObject.FindObjectOfType<Hips>();
				}
				return _hips;
			}
		}

		void Update () 
		{
			transform.position = hips.transform.position - hips.transform.up;

			Vector3 toOrganelle = Vector3.Normalize( organelle.transform.position - transform.position );
			float cosine = Vector3.Dot( transform.up, toOrganelle );
			if (cosine > -1f && cosine < 1f)
			{
				float angle = 180f * Mathf.Acos( cosine ) / Mathf.PI;
				Vector3 axis = Vector3.Normalize( Vector3.Cross( transform.up, toOrganelle ) );
				transform.RotateAround( transform.position, axis, angle );
			}
		}
	}
}
