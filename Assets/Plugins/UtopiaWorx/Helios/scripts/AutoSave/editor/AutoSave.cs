using UnityEngine;
using UnityEditor;
namespace UtopiaWorx.Helios
{
[InitializeOnLoad]
public class AutoSave 
{
	static AutoSave()
	{
		EditorApplication.playmodeStateChanged += SaveCurrentScene;
	}

	static void SaveCurrentScene()
	{
		if( !EditorApplication.isPlaying 
			&& EditorApplication.isPlayingOrWillChangePlaymode )
		{
			UnityEngine.Debug.Log("Saving Scene");
			UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
		}
	}
}
}