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
	public float maxUpdateInterval = 1f;

	float lastTime = -1;
	float lastDistance = 20f;

	void Update () 
	{
		if (Time.time - lastTime > maxUpdateInterval * lastDistance / 20f) // should be more frequent when near a LOD boundary instead of when near the camera
		{
			CheckLOD();
			lastTime = Time.time;
		}
	}

	void CheckLOD ()
	{
		lastDistance = Vector3.Distance( transform.position, Camera.main.transform.position );

		if (lastDistance > lods[lods.Length - 1].maxDistance)
		{
			if (currentLOD != -1)
			{
				SwitchLOD( -1 );
			}
			return;
		}

		for (int i = 0; i < lods.Length; i++)
		{
			if (lods[i].maxDistance <= 0 || lastDistance < lods[i].maxDistance)
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
		if (currentLOD >= 0)
		{
			lods[currentLOD].geometry.SetActive( false );
		}
		if (newLOD >= 0)
		{
			lods[newLOD].geometry.SetActive( true );
		}
		currentLOD = newLOD;
	}
}
