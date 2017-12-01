using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyObjects : MonoBehaviour 
{
	public string[] namesOfObjectsToDestroy;

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	void OnLevelFinishedLoading (Scene scene, LoadSceneMode mode)
	{
		Debug.Log( scene.name + " loaded" );
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
