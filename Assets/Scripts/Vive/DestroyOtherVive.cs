using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOtherVive : MonoBehaviour 
{
	SteamVR_PlayArea playArea;


	void Start () 
	{
		SteamVR_PlayArea[] vives = GetComponents<SteamVR_PlayArea>();
		for (int i = 0; i < 2; i++)
		{
			if (i > vives.Length - 1)
			{
				return;
			}
			if (vives[i].gameObject != gameObject)
			{
				Destroy( vives[i].gameObject );
			}
		}
	}
}
