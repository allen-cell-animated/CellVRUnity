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

	float lastTime = -1;

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
		float cameraDistance = Vector3.Distance( transform.position, Camera.main.transform.position );

		if (cameraDistance > lods[lods.Length - 1].maxDistance)
		{
			if (currentLOD != -1)
			{
				SwitchLOD( -1 );
			}
			return;
		}

		for (int i = 0; i < lods.Length; i++)
		{
			if (lods[i].maxDistance <= 0 || cameraDistance < lods[i].maxDistance)
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
