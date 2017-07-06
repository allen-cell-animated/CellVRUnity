using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Tropomyosin : MonoBehaviour 
	{
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
			transform.position = hips.transform.position;
		}
	}
}
