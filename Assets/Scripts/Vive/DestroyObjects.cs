using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjects : MonoBehaviour 
{
	public string[] namesOfObjectsToDestroy;

	static DestroyObjects _Instance;
	public static DestroyObjects Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = GameObject.FindObjectOfType<DestroyObjects>();
			}
			return _Instance;
		}
	}

	public void DoDestroy () 
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
