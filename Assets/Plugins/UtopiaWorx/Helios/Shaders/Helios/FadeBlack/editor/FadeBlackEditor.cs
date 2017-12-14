using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UtopiaWorx;
using UtopiaWorx.Helios;

namespace UtopiaWorx.Helios.Effects
{
	[CustomEditor(typeof(FadeBlack))]
	public class FadeBlackEditor : Editor 
	{

		public override void OnInspectorGUI()
		{

			serializedObject.Update();

			SerializedProperty sp_Amount = serializedObject.FindProperty ("Amount");
			SerializedProperty sp_SourceMaterial = serializedObject.FindProperty ("SourceMaterial");
			SerializedProperty sp_FadeColor = serializedObject.FindProperty ("FadeColor");

			
			EditorGUI.BeginChangeCheck();
			sp_SourceMaterial.objectReferenceValue = EditorGUILayout.ObjectField("",sp_SourceMaterial.objectReferenceValue,typeof(Material),true) as Material;
			sp_Amount.floatValue =  EditorGUILayout.Slider("Amount",sp_Amount.floatValue,0.0f,1.0f);
			sp_FadeColor.colorValue = EditorGUILayout.ColorField(sp_FadeColor.colorValue);

			if(EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}