using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public class AmbientSprite : MonoBehaviour 
	{
		public bool updateEveryFrame = false;
		public float updateInterval = 1f;

		float lastTime;

		void Start () 
		{
//			Vector3 toCamera = Camera.main.transform.position - transform.position;
//			transform.rotation = Quaternion.AngleAxis( Random.Range( 0, 359f ), toCamera ) * Quaternion.LookRotation( toCamera );
			lastTime = Time.time;
		}

		public void DoUpdate ()
		{
			if (updateEveryFrame || (Time.time - lastTime >= updateInterval))
			{
				LookAtCamera();
				lastTime = Time.time;
			}
		}

		void LookAtCamera ()
		{
			transform.LookAt( Camera.main.transform.position );
		}
	}
}