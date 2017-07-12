using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS;

public class AngleToleranceTester : MonoBehaviour 
{
	public float tolerance = 30f;
	public Transform referenceTransform;

	Vector3 goalRotation = new Vector3( 357.7f, 269.5f, 180.2f );

	MeshRenderer _meshRenderer;
	MeshRenderer meshRenderer
	{
		get {
			if (_meshRenderer == null)
			{
				_meshRenderer = GetComponentInChildren<MeshRenderer>();
			}
			return _meshRenderer;
		}
	}

	void Start ()
	{
		Debug.Log( (Quaternion.Inverse( referenceTransform.rotation ) * transform.rotation).eulerAngles );
	}

	void Update ()
	{
		if (referenceTransform != null)
		{
			Vector3 localRotation = (Quaternion.Inverse( referenceTransform.rotation ) * transform.rotation).eulerAngles;
			if (Helpers.AngleIsWithinTolerance( localRotation.x, goalRotation.x, tolerance ) &&
				Helpers.AngleIsWithinTolerance( localRotation.y, goalRotation.y, tolerance ) &&
				Helpers.AngleIsWithinTolerance( localRotation.z, goalRotation.z, tolerance ))
			{
				meshRenderer.material.color = Color.green;
			}
			else 
			{
				meshRenderer.material.color = Color.red;
			}
		}
	}
}
