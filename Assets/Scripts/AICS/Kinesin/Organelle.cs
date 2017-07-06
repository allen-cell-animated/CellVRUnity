using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Organelle : MonoBehaviour 
	{
		float startDistance;

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

		void Start ()
		{
			startDistance = Vector3.Distance( hips.transform.position, transform.position );
		}
		
		void Update () 
		{
			FollowHips();
		}

		void FollowHips ()
		{
			Vector3 hipsToOrganelle = transform.position - hips.transform.position;
			transform.position = hips.transform.position + startDistance * Vector3.Normalize( hipsToOrganelle );
		}
	}
}