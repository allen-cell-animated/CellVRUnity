using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public static class Helpers 
	{
		public static Vector3 GetRandomVector (float magnitude)
		{
//			Vector3 random = new Vector3( Random.Range( -1f, 1f ), Random.Range( -1f, 1f ), Random.Range( -1f, 1f ) );
			return magnitude * Random.onUnitSphere;
		}
	}
}