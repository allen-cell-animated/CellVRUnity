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
					List<Link> linkList = new List<Link>();
					for (int i = 0; i < joints.Length; i++)
					{
						Link link = joints[i].connectedBody.GetComponent<Link>();
						if (link != null)
						{
							linkList.Add( link );
						}
					}
					_links = linkList.ToArray();
				}
				return _links;
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
					if (!links[i].neckLinker.motor.releasing) { Debug.Log("hips released " + links[i].neckLinker.motor.name); }
					links[i].neckLinker.motor.Release();
				}
			}
		}
	}
}