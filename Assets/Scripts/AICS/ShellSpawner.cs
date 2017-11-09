using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public class ShellSpawner : MonoBehaviour 
	{
		public GameObject prefab;
		public int number = 10;
		public float shellRadius = 50f;
		public float crowding = 0.75f;
		public Vector2 scaleRange = new Vector2( 500f, 1000f );

		void Start () 
		{
			SpawnAll();
		}

		void SpawnAll ()
		{
			List<Vector3> positions = GetPointsOnSphere( Mathf.RoundToInt( number / crowding ) );
			for (int i = 0; i < number; i++)
			{
				int index = Random.Range( 0, positions.Count );
				SpawnObject( positions[index] );
				positions.RemoveAt( index );
			}
		}

		void SpawnObject (Vector3 position)
		{
			GameObject obj = Instantiate( prefab, transform );
			obj.transform.localPosition = position;
			obj.transform.localScale = Random.Range( scaleRange.x, scaleRange.y ) * Vector3.one;
		}

		List<Vector3> GetPointsOnSphere (int n)
		{
			List<Vector3> points = new List<Vector3>();
			float inc = Mathf.PI * (3f - Mathf.Sqrt( 5f ));
			float off = 2f / n;
			float x = 0, y = 0, z = 0, theta = 0, phi = 0;

			for (var k = 0; k < n; k++)
			{
				phi = k * inc;
				theta = k * inc / 2f;
				x = Mathf.Sin( theta ) * Mathf.Cos( phi );
				y = Mathf.Sin( theta ) * Mathf.Sin( phi );
				z = Mathf.Cos( theta );

				points.Add( shellRadius * new Vector3( x, y, z ) );
			}
			return points;
		}
	}
}