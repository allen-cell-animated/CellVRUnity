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

		Rigidbody[] _tropomyosinSegments;
		Rigidbody[] tropomyosinSegments
		{
			get {
				if (_tropomyosinSegments == null)
				{
					SetTropomyosinAndCargo();
				}
				return _tropomyosinSegments;
			}
		}

		Rigidbody _cargo;
		Rigidbody cargo
		{
			get {
				if (_cargo == null)
				{
					SetTropomyosinAndCargo();
				}
				return _cargo;
			}
		}

		MoleculeGenerator _moleculeGenerator;
		MoleculeGenerator moleculeGenerator
		{
			get {
				if (_moleculeGenerator == null)
				{
					_moleculeGenerator = GetComponent<MoleculeGenerator>();
				}
				return _moleculeGenerator;
			}
		}

		void SetTropomyosinAndCargo ()
		{
			Rigidbody b = null;
			for (int i = 0; i < joints.Length; i++)
			{
				if (joints[i].connectedBody != null && joints[i].connectedBody.GetComponent<Link>() == null)
				{
					b = joints[i].connectedBody;
					break;
				}
			}

			List<Rigidbody> segments = new List<Rigidbody>();
			while (b != null && b.GetComponent<PhysicsOrganelle>() == null)
			{
				segments.Add( b );
				b = b.GetComponent<ConfigurableJoint>().connectedBody;
			}
			_tropomyosinSegments = segments.Count > 0 ? segments.ToArray() : null;
			_cargo = b;
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

		public void SetTropomyosinMass (float mass)
		{
			foreach (Rigidbody segment in tropomyosinSegments)
			{
				segment.mass = mass;
			}
		}

		public void SetCargoMass (float mass)
		{
			cargo.mass = mass;
		}

		public void SetATPMass (float mass)
		{
			moleculeGenerator.SetMoleculeMass( mass );
		}
	}
}