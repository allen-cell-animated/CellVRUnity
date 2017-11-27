using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MT;

namespace AICS.MotorProteins.Kinesin
{
	public class MeshSpawner : MonoBehaviour 
	{
		public GameObject prefab;
		public Spline spline;
		public int number;
		public float scale;
		public Vector2 tRange;

		void Start ()
		{
			float t = 0, dT = (1f - (tRange.y - tRange.x)) / (float)number;
			for (int i = 0; i < number; i++)
			{
				if (t <= tRange.x || t >= tRange.y)
				{
					Spawn( t );
					t += dT;
				}
				else 
				{
					t = tRange.y;
				}
			}
		}

		void Spawn (float t)
		{
//			Vector3 tangent = spline.GetTangent( t );
//			Vector3 normal = spline.GetNormal( t );
			Vector3 position = spline.GetPosition( t );
//			Debug.Log( position );
			Quaternion rotation = Quaternion.Euler( new Vector3( -90f, 90f, 90f ) );// * Quaternion.LookRotation( normal, position + tangent );

			GameObject obj = Instantiate( prefab, transform ) as GameObject;
			obj.transform.position = new Vector3( 0, 0, position.z );
			obj.transform.rotation = rotation;
			obj.transform.localScale = scale * Vector3.one;
		}
	}
}