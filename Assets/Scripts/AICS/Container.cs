using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public class Container : MonoBehaviour 
	{
		public Vector3 size = 100f * Vector3.one;
		public float wallWidth = 100f;

		List<Transform> _walls;
		List<Transform> walls
		{
			get
			{
				if (_walls == null || _walls.Count < 6 || _walls.Contains( null ))
				{
					_walls = new List<Transform>();
					BoxCollider[] extantWalls = GetComponentsInChildren<BoxCollider>();
					for (int i = 0; i < 6; i++)
					{
						if (i < extantWalls.Length)
						{
							_walls.Add( extantWalls[i].transform );
						}
						else
						{
							Transform newWall = GameObject.CreatePrimitive( PrimitiveType.Cube ).transform;
							newWall.GetComponent<MeshRenderer>().enabled = false;
							newWall.parent = transform;
							_walls.Add( newWall );
						}
					}
				}
				return _walls;
			}
		}

		void OnDrawGizmos ()
		{
			Gizmos.DrawWireCube( transform.position, size );
		}

		void Start ()
		{
			SetWalls();
		}

		public void SetWalls ()
		{
			int w = 0;
			Vector3 extents = size + 2f * wallWidth * Vector3.one;
			Vector3 position, scale;
			for (int axis = 0; axis < 3; axis++)
			{
				position = Vector3.zero;
				position[axis] = (size[axis] + wallWidth) / 2f;

				scale = extents;
				scale[axis] = wallWidth;

				for (int i = 0; i < 2; i++)
				{
					SetWall( w, i == 0 ? position : -position, scale );
					w++;
				}
			}
		}

		void SetWall (int index, Vector3 position, Vector3 scale)
		{
			walls[index].localPosition = position;
			walls[index].localScale = scale;
			walls[index].name = "wall" + (index + 1);
		}
	}
}