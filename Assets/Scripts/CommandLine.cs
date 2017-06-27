using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class CommandLine 
{
	public static void Test ()
	{
		Debug.Log( "Hello!" );
		EditorSceneManager.OpenScene( "Assets/Scenes/necklinker.unity" );
		EditorApplication.isPlaying = true;
		Debug.Log( "hips mass = " + GameObject.Find( "Hips").GetComponent<Rigidbody>().mass );
	}
}
