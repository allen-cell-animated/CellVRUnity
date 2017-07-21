using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class JointTest : MonoBehaviour 
{
	float startDistanceToAnchor;
	public float distanceToAnchor;

	CharacterJoint _joint;
	CharacterJoint joint
	{
		get {
			if (_joint == null)
			{
				_joint = GetComponent<CharacterJoint>();
			}
			return _joint;
		}
	}

	MeshRenderer _meshRenderer;
	MeshRenderer meshRenderer
	{
		get {
			if (_meshRenderer == null)
			{
				_meshRenderer = GetComponent<MeshRenderer>();
			}
			return _meshRenderer;
		}
	}

	void Start ()
	{
		startDistanceToAnchor = Vector3.Distance( joint.connectedBody.transform.position, transform.position );
	}

	void Update ()
	{
		distanceToAnchor = Vector3.Distance( joint.connectedBody.transform.position, transform.position ) / startDistanceToAnchor;
		if (distanceToAnchor < 1f)
		{
			meshRenderer.material.color = Color.green;
		}
		else if (distanceToAnchor < 1.1f)
		{
			meshRenderer.material.color = Color.yellow;
		}
		else
		{
			meshRenderer.material.color = Color.red;
//			EditorApplication.isPaused = true;
		}
	}
}
