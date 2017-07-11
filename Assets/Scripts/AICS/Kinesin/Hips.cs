using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Hips : MonoBehaviour 
	{
		Rigidbody _body;
		Rigidbody body
		{
			get {
				if (_body == null)
				{
					_body = GetComponent<Rigidbody>();
				}
				return _body;
			}
		}

		ConfigurableJoint[] _joints;
		ConfigurableJoint[] joints
		{
			get {
				if (_joints == null)
				{
					_joints = GetComponents<ConfigurableJoint>();
				}
				return _joints;
			}
		}

		public void AttachToMotors (Rigidbody[] motorLastLinks)
		{
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

		public void SetMass (float mass)
		{
			body.mass = mass;
		}

		public void SetJointRotationLimits (Vector3 newLimits)
		{
			foreach (ConfigurableJoint joint in joints)
			{
				Helpers.SetJointRotationLimits( joint, newLimits );
			}
		}
	}
}