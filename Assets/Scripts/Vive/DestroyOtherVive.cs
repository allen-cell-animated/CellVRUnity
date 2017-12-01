using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOtherVive : MonoBehaviour 
{
	void Start () 
	{
		SteamVR_PlayArea[] vives = GetComponents<SteamVR_PlayArea>();
		Debug.Log( "found " + vives.Length + " vives" );
		for (int i = 0; i < 10; i++)
		{
			if (i > vives.Length - 1)
			{
				return;
			}
			if (vives[i].gameObject != gameObject)
			{
				Debug.Log( "destroying vive #" + i );
				Destroy( vives[i].gameObject );
			}
		}
	}
}
