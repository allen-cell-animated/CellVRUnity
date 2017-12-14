using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace UtopiaWorx.Helios
{
	[CustomEditor(typeof(UtopiaWorx.Helios.HeliosSessionRecorder))]
	public class SessionRecorderEditor : Editor 
	{
			
			void OnDisable() 
			{

			}
		public override void OnInspectorGUI()
		{

			if(UnityEditor.EditorApplication.isPlaying == false)
			{
			UtopiaWorx.Helios.HeliosSessionRecorder myTarget = (UtopiaWorx.Helios.HeliosSessionRecorder)target;

				string[] CursorStates = {"Confined","Locked","None"};

			serializedObject.Update();


				SerializedProperty sp_CursorState = serializedObject.FindProperty ("CursorState");

			SerializedProperty sp_Frame = serializedObject.FindProperty ("Frame");
			SerializedProperty sp_SaveSession = serializedObject.FindProperty ("SaveSession");
			SerializedProperty sp_PegasusMode = serializedObject.FindProperty ("PegasusMode");
			SerializedProperty sp_SessionName = serializedObject.FindProperty ("SessionName");
				SerializedProperty sp_EnablePlayback = serializedObject.FindProperty ("EnablePlayback");


			SerializedProperty sp_CaptureMouseHoriz = serializedObject.FindProperty ("CaptureMouseHoriz");
			SerializedProperty sp_CaptureMouseVert = serializedObject.FindProperty ("CaptureMouseVert");
			SerializedProperty sp_CaptureHoriz = serializedObject.FindProperty ("CaptureHoriz");
			SerializedProperty sp_CaptureVert = serializedObject.FindProperty ("CaptureVert");
			SerializedProperty sp_SaveAudio = serializedObject.FindProperty ("SaveAudio");

			 




			EditorGUI.BeginChangeCheck();

				if(EditorApplication.isPlaying == true)
				{
					if(sp_SaveSession.boolValue == true)
					{
						EditorGUILayout.LabelField("Recording Frame : " + sp_Frame.intValue.ToString());
					}
					else
					{
						EditorGUILayout.LabelField("Transmitting Frame : " + sp_Frame.intValue.ToString());					
					}
				}

				sp_SaveSession.boolValue =  EditorGUILayout.Toggle(new GUIContent("Save Session","Record the movement data of your Helios rig and save it to be played back later."),sp_SaveSession.boolValue);
				sp_PegasusMode.boolValue =  EditorGUILayout.Toggle(new GUIContent("Pegasus Mode","You can use this mode to record Audio only and not position vectors."),sp_PegasusMode.boolValue);
				sp_EnablePlayback.boolValue =  EditorGUILayout.Toggle(new GUIContent("Enable Playback","should you play back this track."),sp_EnablePlayback.boolValue);

			if(sp_SaveSession.boolValue == true) 
			{
					try
					{
						myTarget.gameObject.GetComponent<UtopiaWorx.Helios.Helios>().IsArmed = false;
					}
					catch
					{}

					try
					{
						myTarget.gameObject.GetComponent<UtopiaWorx.Helios.Helios2D>().IsArmed = false;
					}
					catch
					{}
			}

				sp_SaveAudio.boolValue =  EditorGUILayout.Toggle(new GUIContent("Save Audio","Save the audio stream from your main Audio Listener to a file which can be fed into Helios later as a soundtrack for your movie."),sp_SaveAudio.boolValue);

				if(myTarget.gameObject.GetComponent<Helios2D>() != null)
				{


				sp_CaptureMouseHoriz.boolValue =  EditorGUILayout.Toggle(new GUIContent("Mouse X","Capture Mouse X Axis"),sp_CaptureMouseHoriz.boolValue);
				sp_CaptureMouseVert.boolValue =  EditorGUILayout.Toggle(new GUIContent("Mouse Y","Capture Mouse Y Axis"),sp_CaptureMouseVert.boolValue);
				}	
				sp_CaptureHoriz.boolValue =  EditorGUILayout.Toggle(new GUIContent("Horizontal Movement","Capture Left/Right (A & D)"),sp_CaptureHoriz.boolValue);
					sp_CaptureVert.boolValue =  EditorGUILayout.Toggle(new GUIContent("Vertical Movement","Capture Front/Back (W & S)"),sp_CaptureVert.boolValue);
				sp_CursorState.intValue =  EditorGUILayout.Popup("Cursor Lock",sp_CursorState.intValue, CursorStates);


				sp_SessionName.stringValue = EditorGUILayout.TextField(new GUIContent("Session Name:","Once you complete recording your session, Helios will crate a file with this name to read from later."),sp_SessionName.stringValue);
			if(sp_SessionName.stringValue == "")
			{
				sp_SessionName.stringValue = "MySession";
			}
				if(sp_SessionName.stringValue.Contains(" ") == true)
				{
					sp_SessionName.stringValue = sp_SessionName.stringValue.Replace(" ","_");
					EditorUtility.DisplayDialog("Session Name","Session Names should not contain Spaces or any special characters, Helios has replaced spaces with Underscores","OK");

				}
				if(GUILayout.Button("Clear Session Data"))
				{
					if(System.IO.File.Exists(Application.dataPath + "/UtopiaWorx/Helios/Sessions/" + sp_SessionName.stringValue + ".txt") == true)
					{
						System.IO.File.Delete(Application.dataPath + "/UtopiaWorx/Helios/Sessions/" + sp_SessionName.stringValue + ".txt");
					}
				}

					if(UtopiaWorx.Helios.HeliosSessionRecorder.RecorderState == 2)
					{
			if(System.IO.File.Exists(Application.dataPath + "/UtopiaWorx/Helios/Sessions/" + sp_SessionName.stringValue + ".wav") == true)
			{
				AudioClip TheClip = (AudioClip)AssetDatabase.LoadAssetAtPath( "Assets/UtopiaWorx/Helios/Sessions/" + sp_SessionName.stringValue  + ".wav",typeof(AudioClip));
				
						try
						{
							myTarget.gameObject.GetComponent<UtopiaWorx.Helios.Helios>().Soundtrack =TheClip;
						}
						catch
						{}
						try
						{
							myTarget.gameObject.GetComponent<UtopiaWorx.Helios.Helios2D>().Soundtrack =TheClip;
						}
						catch
						{}
				UtopiaWorx.Helios.HeliosSessionRecorder.RecorderState = 1;
			}
				}




			if(EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
		}
		}
	}
}