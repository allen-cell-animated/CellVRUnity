using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.MotorProteins.Kinesin
{
	public class Follower : MonoBehaviour 
	{
		public Transform transformToFollow;
		public bool followPosition = true;
		public bool followRotation = true;

		public void Follow () 
		{
			if (followPosition)
			{
				transform.position = transformToFollow.position;
			}
			if (followRotation)
			{
				transform.rotation = transformToFollow.rotation;
			}
		}
	}
}