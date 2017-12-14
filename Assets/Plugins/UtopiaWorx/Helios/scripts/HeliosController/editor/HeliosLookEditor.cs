using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace UtopiaWorx.Helios
{
[CustomEditor(typeof(UtopiaWorx.Helios.HeliosLook))]
public class HeliosLookEditor : Editor 
{

	public override void OnInspectorGUI()
	{


			if(UnityEditor.EditorApplication.isPlaying == false)
			{


		serializedObject.Update();

		SerializedProperty sp_sensitivityX = serializedObject.FindProperty ("sensitivityX");
		SerializedProperty sp_sensitivityY = serializedObject.FindProperty ("sensitivityY");
			SerializedProperty sp_BypassMode = serializedObject.FindProperty ("BypassMode");





		EditorGUI.BeginChangeCheck();
			sp_BypassMode.boolValue = EditorGUILayout.Toggle("Bypass",sp_BypassMode.boolValue);
			if(sp_BypassMode.boolValue == false)
			{
		sp_sensitivityX.floatValue =  EditorGUILayout.Slider("X Sensitivity",sp_sensitivityX.floatValue,1.0f,20.0f);
		sp_sensitivityY.floatValue =  EditorGUILayout.Slider("Y Sensitivity",sp_sensitivityY.floatValue,1.0f,20.0f);
			}



		if(EditorGUI.EndChangeCheck())
		{
			serializedObject.ApplyModifiedProperties();
		}
	}
		}
}
}