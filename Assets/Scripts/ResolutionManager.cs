using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LOD
{
	public float maxDistance = 0;
	public GameObject geometry;
}

public class ResolutionManager : MonoBehaviour 
{
	public int currentLOD = 0;
	public LOD[] lods = new LOD[2];
	public float updateInterval = 1f;

	float lastTime = 0;

	void Update () 
	{
		if (Time.time - lastTime > updateInterval)
		{
			CheckLOD();
			lastTime = Time.time;
		}
	}

	void CheckLOD ()
	{
		for (int i = 0; i < lods.Length; i++)
		{
			if (lods[i].maxDistance <= 0 || Vector3.Distance( transform.position, Camera.main.transform.position ) < lods[i].maxDistance)
			{
				if (currentLOD != i)
				{
					SwitchLOD( i );
				}
				return;
			}
		}
	}

	void SwitchLOD (int newLOD)
	{
		lods[currentLOD].geometry.SetActive( false );
		lods[newLOD].geometry.SetActive( true );
		currentLOD = newLOD;
	}
}
