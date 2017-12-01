using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjects : MonoBehaviour 
{
	public string[] namesOfObjectsToDestroy;

	void Start () 
	{
		for (int i = 0; i < namesOfObjectsToDestroy.Length; i++)
		{
			GameObject obj = GameObject.Find( namesOfObjectsToDestroy[i] );
			if (obj != null)
			{
				Debug.Log( "destroying " + obj.name );
				Destroy( obj );
			}
		}
	}
}
