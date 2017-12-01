using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyObjects : MonoBehaviour 
{
	public string[] namesOfObjectsToDestroy;

	void OnEnable()
	{
		SteamVR_Events.Loading += OnLevelFinishedLoading;
	}

	void OnDisable()
	{
		SteamVR_Events.Loading -= OnLevelFinishedLoading;
	}

	void OnLevelFinishedLoading (bool loading)
	{
		if (!loading)
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
}
