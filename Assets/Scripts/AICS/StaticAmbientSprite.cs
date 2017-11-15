using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public class StaticAmbientSprite : MonoBehaviour 
	{
		public bool animated = false;

		void Start () 
		{
			Vector3 toCamera = Camera.main.transform.position - transform.position;
			transform.rotation = Quaternion.AngleAxis( Random.Range( 0, 359f ), toCamera ) * Quaternion.LookRotation( toCamera );

			if (animated)
			{
				Animator animator = GetComponentInChildren<Animator>();
				animator.Play( Random.value < 0.5f ? "forward" : "reverse", -1, Random.value );
			}
		}
	}
}