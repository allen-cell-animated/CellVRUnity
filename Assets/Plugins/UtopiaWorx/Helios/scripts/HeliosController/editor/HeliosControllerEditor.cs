using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace UtopiaWorx.Helios
{
[CustomEditor(typeof(UtopiaWorx.Helios.HeliosController))]
public class HeliosControllerEditor : Editor 
{

	public override void OnInspectorGUI()
	{
			if(UnityEditor.EditorApplication.isPlaying == false)
			{
		serializedObject.Update();

		SerializedProperty sp_HorizontalSpeed = serializedObject.FindProperty ("HorizontalSpeed");
			SerializedProperty sp_JumpVelocity = serializedObject.FindProperty ("JumpVelocity");
			SerializedProperty sp_BypassMode = serializedObject.FindProperty ("BypassMode");





		EditorGUI.BeginChangeCheck();
			sp_BypassMode.boolValue = EditorGUILayout.Toggle("Bypass",sp_BypassMode.boolValue);
			if(sp_BypassMode.boolValue == false)
			{
				sp_HorizontalSpeed.floatValue =  EditorGUILayout.Slider("Horizontal Speed",sp_HorizontalSpeed.floatValue,0.1f,2.0f);
				sp_JumpVelocity.floatValue =  EditorGUILayout.Slider("Jump Velocity",sp_JumpVelocity.floatValue,200.0f,2000.0f);
			}



		if(EditorGUI.EndChangeCheck())
		{
			serializedObject.ApplyModifiedProperties();
		}
	}
		}
}
}