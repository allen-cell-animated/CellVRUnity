using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour 
{
	void Update () 
	{
		LookAtCamera();
	}

	public void LookAtCamera ()
	{
		transform.LookAt(Camera.main.transform.position);
	}
}
