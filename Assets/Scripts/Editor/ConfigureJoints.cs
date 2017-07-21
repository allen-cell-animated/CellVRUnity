using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConfigureJoints
{
	[MenuItem("AICS/Configure Joint Axes")]
	static void ConfigureAxes () 
	{
		List<ConfigurableJoint> roots = GetRoots();
		foreach (ConfigurableJoint root in roots)
		{
			SetupChain( root );
		}
		Debug.Log( "Joint axes configured" );
	}

	static List<ConfigurableJoint> GetRoots ()
	{
		ConfigurableJoint[] joints = GameObject.FindObjectsOfType<ConfigurableJoint>();
		List<ConfigurableJoint> roots = new List<ConfigurableJoint>();
		foreach (ConfigurableJoint joint in joints)
		{
			if (JointIsARoot( joint, joints ))
			{
				roots.Add( joint );
			}
		}
		return roots;
	}

	static bool JointIsARoot (ConfigurableJoint joint, ConfigurableJoint[] joints)
	{
		foreach (ConfigurableJoint otherJoint in joints)
		{
			if (otherJoint.connectedBody == joint.GetComponent<Rigidbody>())
			{
				return false;
			}
		}
		return true;
	}

	static void SetupChain (ConfigurableJoint root)
	{
		Transform axisTransform = new GameObject( "axis" ).transform;

		ConfigurableJoint joint = root;
		while (joint != null)
		{
			Vector3 toConnectedBody = Vector3.Normalize( joint.connectedBody.transform.position - joint.transform.position );
			float angle = 180f * Mathf.Acos( Vector3.Dot( axisTransform.forward, toConnectedBody ) ) / Mathf.PI;
			Vector3 axis = Vector3.Normalize( Vector3.Cross( axisTransform.forward, toConnectedBody ) );
			axisTransform.RotateAround( joint.transform.position, axis, angle );

			joint.axis = joint.transform.InverseTransformVector( axisTransform.forward );
			joint.secondaryAxis = joint.transform.InverseTransformVector( axisTransform.up );

			joint = joint.connectedBody.GetComponent<ConfigurableJoint>();
		}

		GameObject.DestroyImmediate( axisTransform.gameObject );
	}
}
