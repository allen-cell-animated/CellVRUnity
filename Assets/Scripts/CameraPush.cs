using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPush : MonoBehaviour 
{
	public float minZ = -10f;
	public float maxZ = -1.01f;
	public float speed = 0.1f;

	void Update () 
	{
		float zoom = transform.position.z + speed * Input.mouseScrollDelta.y;
		transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(zoom, minZ, maxZ));
	}
}
