using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace UtopiaWorx.Helios
{
	[CustomEditor(typeof(UtopiaWorx.Helios.HeliosSessionManager))]
	public class MasterSessionRecorder_editor : Editor 
	{
		
		public override void OnInspectorGUI()
		{
			if(UnityEditor.EditorApplication.isPlaying == false)
			{
			UtopiaWorx.Helios.HeliosSessionManager myTarget = (UtopiaWorx.Helios.HeliosSessionManager)target;

			string[] Modes = {"Active","Passive"};

			//serializedObject.Update();


			//SerializedProperty sp_ManagedGameObjects = serializedObject.FindProperty ("ManagedGameObjects");

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.LabelField("Session Manager");

			if(GUILayout.Button("Add Managed Object"))
			{
				if(myTarget.ManagedGameObjects == null)
				{
					myTarget.ManagedGameObjects = new List<ManagedObject>();
				}
				myTarget.ManagedGameObjects.Add(new ManagedObject());
			}
			if(GUILayout.Button("Reset"))
			{
				myTarget.ManagedGameObjects = new List<ManagedObject>();
			}
			if(myTarget.ManagedGameObjects != null && myTarget.ManagedGameObjects.Count > 0)
			{
				
			for(int i =0; i < myTarget.ManagedGameObjects.Count; i++)
			{
					string ObjectName ="";
					try
					{
						ObjectName = myTarget.ManagedGameObjects[i].TargetGameObject.name;
					}
					catch
					{
						ObjectName = "Unassigned";
					}
					myTarget.ManagedGameObjects[i].IsCollapsed = EditorGUILayout.Foldout(myTarget.ManagedGameObjects[i].IsCollapsed,ObjectName);
						
				if(myTarget.ManagedGameObjects[i].IsCollapsed == false)
				{
					try
					{
						myTarget.ManagedGameObjects[i].TargetGameObject = (GameObject)EditorGUILayout.ObjectField(myTarget.ManagedGameObjects[i].TargetGameObject,typeof(GameObject),true) as GameObject;
					}
					catch
					{
						
					}
					if(myTarget.ManagedGameObjects[i].TargetGameObject != null)
					{
					if(myTarget.ManagedGameObjects[i].TargetGameObject.GetComponent<HeliosSessionRecorder>() == false)
					{
						myTarget.ManagedGameObjects[i].TargetGameObject.AddComponent<HeliosSessionRecorder>();
							myTarget.ManagedGameObjects[i].TargetGameObject.GetComponent<HeliosSessionRecorder>().CaptureHoriz = true;
							myTarget.ManagedGameObjects[i].TargetGameObject.GetComponent<HeliosSessionRecorder>().CaptureVert = true;
							myTarget.ManagedGameObjects[i].TargetGameObject.GetComponent<HeliosSessionRecorder>().SaveAudio = false;
							myTarget.ManagedGameObjects[i].TargetGameObject.GetComponent<HeliosSessionRecorder>().SessionName = System.Guid.NewGuid().ToString();
							myTarget.ManagedGameObjects[i].SessionName =  myTarget.ManagedGameObjects[i].TargetGameObject.GetComponent<HeliosSessionRecorder>().SessionName;
						}
					if(myTarget.ManagedGameObjects[i].TargetGameObject.GetComponent<HeliosController>() == false)
					{
						myTarget.ManagedGameObjects[i].TargetGameObject.AddComponent<HeliosController>();
					}
					if(myTarget.ManagedGameObjects[i].TargetGameObject.GetComponent<HeliosLook>() == false)
					{
						myTarget.ManagedGameObjects[i].TargetGameObject.AddComponent<HeliosLook>();
					}
						EditorGUILayout.BeginVertical();
						myTarget.ManagedGameObjects[i].RecordArmed = EditorGUILayout.Toggle("Record:",myTarget.ManagedGameObjects[i].RecordArmed);
						myTarget.ManagedGameObjects[i].PlaybackArmed = EditorGUILayout.Toggle("Playback:",myTarget.ManagedGameObjects[i].PlaybackArmed);
							if(myTarget.ManagedGameObjects[i].PlaybackArmed == true)
							{
								myTarget.ManagedGameObjects[i].TargetGameObject.GetComponent<HeliosSessionRecorder>().EnablePlayback = true;
							}
							else
							{
								myTarget.ManagedGameObjects[i].TargetGameObject.GetComponent<HeliosSessionRecorder>().EnablePlayback = false;
							}
						myTarget.ManagedGameObjects[i].SessionName = EditorGUILayout.TextField("Session Name:",myTarget.ManagedGameObjects[i].SessionName);
						myTarget.ManagedGameObjects[i].Mode = EditorGUILayout.Popup("Mode:",myTarget.ManagedGameObjects[i].Mode,Modes);
							if(myTarget.ManagedGameObjects[i].Mode == 0)
							{
								myTarget.ManagedGameObjects[i].TargetGameObject.GetComponent<HeliosController>().BypassMode = false;
								myTarget.ManagedGameObjects[i].TargetGameObject.GetComponent<HeliosLook>().BypassMode = false;
							}
							else
							{
								myTarget.ManagedGameObjects[i].TargetGameObject.GetComponent<HeliosController>().BypassMode = true;
								myTarget.ManagedGameObjects[i].TargetGameObject.GetComponent<HeliosLook>().BypassMode = true;
							}
						myTarget.ManagedGameObjects[i].TargetGameObject.GetComponent<HeliosSessionRecorder>().SaveSession = myTarget.ManagedGameObjects[i].RecordArmed;	

							bool HasRigid = true;
							if(myTarget.ManagedGameObjects[i].TargetGameObject.GetComponent<Rigidbody>() == null)
							{
								HasRigid = false;
							}

							if(HasRigid == false)
							{
								EditorGUILayout.HelpBox("There was no rigid body found on this object",MessageType.Warning);
								if(GUILayout.Button("Add Rigid Body"))
								{
									myTarget.ManagedGameObjects[i].TargetGameObject.AddComponent<Rigidbody>();
								}
							}

							bool HasCollider = true;
							if(myTarget.ManagedGameObjects[i].TargetGameObject.GetComponent<Collider>() == null)
							{
								HasCollider = false;
							}

							if(HasCollider == false)
							{
								EditorGUILayout.HelpBox("There was no collider body found on this object",MessageType.Warning);
								if(GUILayout.Button("Add Box Collider"))
								{
									myTarget.ManagedGameObjects[i].TargetGameObject.AddComponent<BoxCollider>();
								}
							}


						EditorGUILayout.EndVertical();
					}
						if(myTarget.ManagedGameObjects[i].TargetGameObject.GetComponent<Rigidbody>() != null)
						{
							EditorGUILayout.HelpBox("Rember to disable any character controller you may have that could be trying to write values to the Animator on this object.",MessageType.Warning);
						}

					if(GUILayout.Button("Remove"))
					{
						myTarget.ManagedGameObjects.RemoveAt(i);
					}
				}
				EditorGUILayout.LabelField("");
			}
			}
//			if(EditorGUI.EndChangeCheck())
//			{
//				serializedObject.ApplyModifiedProperties();
//			}
		}
		}
	}
}
