using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Hips : MonoBehaviour 
	{
		public void AttachToMotors (Rigidbody[] motorLastLinks)
		{
			CharacterJoint[] joints = GetComponents<CharacterJoint>();
			int m = 0;
			for (int i = 0; i < joints.Length; i++)
			{
				if (joints[i].connectedBody == null || joints[i].connectedBody.GetComponent<Link>())
				{
					joints[i].connectedBody = motorLastLinks[m];
					m++;
				}
			}
		}
	}
}