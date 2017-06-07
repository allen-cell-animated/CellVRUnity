using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Kinesin
{
	public class Hips : MonoBehaviour 
	{
		float[] startDistanceToAnchor = new float[2];
		public float[] distanceToAnchor;

		Link[] _links;
		Link[] links
		{
			get {
				if (_links == null)
				{
					CharacterJoint[] joints = GetComponents<CharacterJoint>();
					_links = new Link[joints.Length];
					for (int i = 0; i < joints.Length; i++)
					{
						_links[i] = joints[i].connectedBody.GetComponent<Link>();
					}
				}
				return _links;
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
			startDistanceToAnchor[0] = Vector3.Distance( links[0].transform.position, transform.position );
			startDistanceToAnchor[1] = Vector3.Distance( links[1].transform.position, transform.position );
			distanceToAnchor = new float[2];
		}

		void Update ()
		{
			distanceToAnchor[0] = Vector3.Distance( links[0].transform.position, transform.position ) / startDistanceToAnchor[0];
			distanceToAnchor[1] = Vector3.Distance( links[1].transform.position, transform.position ) / startDistanceToAnchor[1];

			for (int i = 0; i < links.Length; i++)
			{
				if (distanceToAnchor[i] > 1.1f)
				{
					meshRenderer.material.color = Color.red;
					if (!links[i].neckLinker.motor.releasing) { Debug.Log("hips released " + links[i].neckLinker.motor.name); }
					links[i].neckLinker.motor.Release();
				}
			}

			if (distanceToAnchor[0] < 1f && distanceToAnchor[1] < 1f)
			{
				meshRenderer.material.color = Color.green;
			}
			else if (distanceToAnchor[0] <= 1.1f && distanceToAnchor[1] <= 1.1f)
			{
				meshRenderer.material.color = Color.yellow;
			}
		}
	}
}