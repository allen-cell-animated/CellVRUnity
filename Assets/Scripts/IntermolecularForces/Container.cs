using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour 
{
	public Vector3 size;

	void OnDrawGizmos ()
	{
		Gizmos.DrawWireCube( transform.position, size );
	}
}
