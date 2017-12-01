using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void SteamVRLoadEvent ();

public class DestroyObjects : MonoBehaviour 
{
	public string[] namesOfObjectsToDestroy;

	void OnEnable()
	{
		SteamVR_LoadLevel.OnLoad += OnLevelFinishedLoading;
	}

	void OnDisable()
	{
		SteamVR_LoadLevel.OnLoad -= OnLevelFinishedLoading;
	}

	void OnLevelFinishedLoading ()
	{
		Debug.Log( "destroy!" );
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
