using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Hips : MonoBehaviour 
	{
		Rigidbody _body;
		public Rigidbody body
		{
			get {
				if (_body == null)
				{
					_body = GetComponent<Rigidbody>();
				}
				return _body;
			}
		}

		public void AttachToMotors (Rigidbody[] motorLastLinks)
		{
			Joint[] joints = GetComponents<Joint>();
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

		// Unchanged vector components should be < 0
		public void SetJointRotationLimits (Vector3 limits)
		{
			//TODO
		}
	}
}