using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public class AmbientSprite : MonoBehaviour 
	{
		void Start () 
		{
			Vector3 toCamera = Camera.main.transform.position - transform.position;
			transform.rotation = Quaternion.AngleAxis( Random.Range( 0, 359f ), toCamera ) * Quaternion.LookRotation( toCamera );
		}
	}
}