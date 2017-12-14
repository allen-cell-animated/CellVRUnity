using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace UtopiaWorx.Shaders.PostProcessing
{
    [CustomEditor(typeof(UtopiaWorx.Helios.Helios2D))]
    public class Helios2DEditor : Editor
    {
        private bool HeliosDefined = false;
        int QualityClone;
        private Texture2D Logo;
        private Texture2D MakeVideo;

        float LocalStart = 0;
        float Localend = 0;

        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
            {

                pix[i] = col;
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }


        public override void OnInspectorGUI()
        {

            bool FFMExists = System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/Bin");
            bool RenderTexturesExist = System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures");
            if (RenderTexturesExist == false)
            {
                EditorUtility.DisplayDialog("Helios needs to create some files", "In order to keep the installation package size small, Helios does not ship with all of the resources it needs. It will now create some Render Textures and CubeMaps that it needs", "Got It!");
                UtopiaWorx.Helios.Welcome.RebuildResources();
            }

            if (HeliosDefined == false)
            {
                string currBuildSettings = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

                //Check for and inject GAIA_PRESENT
                if (!currBuildSettings.Contains("HELIOS3D"))
                {
                    try
                    {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currBuildSettings + ";HELIOS3D");
                    }
                    catch
                    { }
                }
                HeliosDefined = true;
            }

            Helios.Helios2D MyTarget = (Helios.Helios2D)target;

            Component[] MyComponents = MyTarget.GetComponent<Camera>().GetComponents<Component>();
            foreach (Component MyCompo in MyComponents)
            {
                if (
                    MyCompo.GetType().ToString() == "UnityEngine.Transform" ||
                    MyCompo.GetType().ToString() == "UnityEngine.Camera" ||
                    MyCompo.GetType().ToString() == "UnityEngine.AudioListener" ||
                    MyCompo.GetType().ToString() == "UtopiaWorx.Helios.Helios2D" ||
                    MyCompo.GetType().ToString() == "UtopiaWorx.Helios.HeliosSessionRecorder" ||
                    MyCompo.GetType().ToString() == "UtopiaWorx.Helios.HeliosController" ||
                    MyCompo.GetType().ToString() == "UtopiaWorx.Helios.HeliosLook" ||
                    MyCompo.GetType().ToString() == "UnityEngine.Rigidbody" ||
                    MyCompo.GetType().ToString() == "UnityEngine.CapsuleCollider")
                {

                }
                else
                {
                    bool Shown = EditorPrefs.GetBool("Helios2DComponentWarning");
                    if (Shown == false)
                    {
                        EditorUtility.DisplayDialog("Add Component Warning", "Hey there Cowboy, \r\nIt looks like you are trying to add something to the Helios camera rig, and thats totally cool...\r\nHowever\r\nIf you're looking to add post processsing effects to Helios, you will need to add them to the child object of Helios, called \"Front\" to make them work in your saved images. We won't bug you with this warning again.", "Got It!");
                        EditorPrefs.SetBool("Helios2DComponentWarning", true);
                    }

                    //UnityEngine.Debug.Log(MyCompo.GetType().ToString());					
                }
            }

            GUIStyle gsTestHeader = new GUIStyle();
            gsTestHeader.normal.background = MakeTex(400, 1, new Color(255.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f, 0.25f));


            if (Logo == null)
            {
                Logo = Resources.Load("Images/Helios2D") as Texture2D;
            }
            if (MakeVideo == null)
            {
                MakeVideo = Resources.Load("Images/Record") as Texture2D;
            }


            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(Logo);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("Version: " + UtopiaWorx.Helios.HeliosUtility.HeliosVersion, MessageType.Info);

            string[] QualitySettings = { "Preview (480)", "Mobile (720)", "Desktop (1080)", "DVD (4K)", "VR (8K)", "YouTube Voodoo", "iPad Portrait", "iPhone 5 Portrait", "iPhone 6 Portrait" };
            string[] QualitySettingsCubic = { "256x512", "512x1024", "1024x2048", "2048x4096", "4096x8192" };
           // string[] AntiAliasingLevels = { "None - Faster", "2x", "4x", "8x - Slower" };
            string[] EncodingPresets = { "Ultra Fast (Low Quality)", "Super Fast", "Very Fast", "Faster", "Fast", "Medium", "Slow", "Slower", "Very Slow (High Quality)" };
            string[] CaptureRates = { "9", "15", "24", "25", "30", "60", "90" };
            string[] RecordModes = { "Normal", "Frame Range", "Hot Key", "Remote Control", "Test Frame" };
            string[] OutputFormats = { "MPEG4", "GIF", "AVI", "MOV", "WEBM" };
            string[] StillFormat = { "EXR", "JPEG", "PNG" };

            serializedObject.Update();
            SerializedProperty sp_CapQual = serializedObject.FindProperty("CapQual");

            SerializedProperty sp_OutputFormat = serializedObject.FindProperty("OutputFormat");
            SerializedProperty sp_EncodingPreset = serializedObject.FindProperty("EncodingPreset");
            SerializedProperty sp_GammaBoost = serializedObject.FindProperty("GammaBoost");
            SerializedProperty sp_GammaBoostAmount = serializedObject.FindProperty("GammaAmount");

           // SerializedProperty sp_AntiAliasingLevel = serializedObject.FindProperty("AntiAliasingLevel");
            SerializedProperty sp_CaptureRate = serializedObject.FindProperty("CaptureRate");
            SerializedProperty sp_Quality = serializedObject.FindProperty("Quality");
            SerializedProperty sp_ProgressData = serializedObject.FindProperty("ProgressData");
            SerializedProperty sp_JPEG = serializedObject.FindProperty("JPEG");
            SerializedProperty sp_PNG = serializedObject.FindProperty("UsePNG");

            SerializedProperty sp_FrontCam = serializedObject.FindProperty("FrontCam");
            SerializedProperty sp_Soundtrack = serializedObject.FindProperty("Soundtrack");
            SerializedProperty sp_DebugMode = serializedObject.FindProperty("DebugMode");
            SerializedProperty sp_ClipDuration = serializedObject.FindProperty("ClipDuration");
            SerializedProperty sp_WorkingDir = serializedObject.FindProperty("WorkingDir");
            SerializedProperty sp_IsArmed = serializedObject.FindProperty("IsArmed");
            SerializedProperty sp_DeleteSource = serializedObject.FindProperty("DeleteSource");
            SerializedProperty sp_FoldoutGeneral = serializedObject.FindProperty("FoldoutGeneral");
            SerializedProperty sp_FoldoutOutput = serializedObject.FindProperty("FoldoutOutput");

            SerializedProperty sp_FoldoutAdvanced = serializedObject.FindProperty("FoldoutAdvanced");
            SerializedProperty sp_FadeInTime = serializedObject.FindProperty("FadeInTime");
            SerializedProperty sp_FadeOutTime = serializedObject.FindProperty("FadeOutTime");
            SerializedProperty sp_SupportMultiScene = serializedObject.FindProperty("SupportMultiScene");

            SerializedProperty sp_GrainStrength = serializedObject.FindProperty("GrainStrength");


            SerializedProperty sp_TheFrame = serializedObject.FindProperty("TheFrame");
            SerializedProperty sp_JPEG_Quality = serializedObject.FindProperty("JPEG_Quality");

            SerializedProperty sp_CinematicAspects = serializedObject.FindProperty("CinematicAspects");

            SerializedProperty sp_RecordMode = serializedObject.FindProperty("RecordMode");

            SerializedProperty sp_StartFrame = serializedObject.FindProperty("StartFrame");
            SerializedProperty sp_EndFrame = serializedObject.FindProperty("EndFrame");
            SerializedProperty sp_HotKey = serializedObject.FindProperty("HotKey");
            SerializedProperty sp_FileFormat = serializedObject.FindProperty("FileFormat");
            SerializedProperty sp_CopyFOV = serializedObject.FindProperty("CopyFOV");

            SerializedProperty sp_UseChromaKey = serializedObject.FindProperty("UseChromaKey");
            SerializedProperty sp_ChromaColor = serializedObject.FindProperty("ChromaColor");

            SerializedProperty sp_ChromaSky = serializedObject.FindProperty("ChromaSky");
            SerializedProperty sp_ChromaBlend = serializedObject.FindProperty("ChromaBlend");

            SerializedProperty sp_FoldoutDiags = serializedObject.FindProperty("FoldoutDiags");

            SerializedProperty sp_UseEXR = serializedObject.FindProperty("UseEXR");
            SerializedProperty sp_StreamToFFMPEG = serializedObject.FindProperty("StreamToFFMPEG");
            SerializedProperty sp_AdditionalSwitches = serializedObject.FindProperty("AdditionalSwitches");

#if UNITY_2017_1_OR_NEWER
            SerializedProperty sp_FrameRateWarning = serializedObject.FindProperty("FrameRateWarning");
            SerializedProperty sp_VideoFrameRates = serializedObject.FindProperty("VideoFrameRates");
#endif







            EditorGUI.BeginChangeCheck();

            GUIStyle GSBold = new GUIStyle();
            GSBold.normal.textColor = Color.yellow;
            GSBold.fontStyle = FontStyle.Normal;
            if (EditorApplication.isPlaying == true)
            {
                EditorGUILayout.BeginHorizontal();

#if UNITY_2017_1_OR_NEWER
                if (sp_FrameRateWarning.boolValue == true)
                {
                    EditorGUILayout.LabelField("Warning, Your Video Players and Helios frame rates do not match.", GSBold);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Helios will attempt to control the playback speed, but there could be side effects", GSBold);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("For best results either adjust your capture frame rate or re-export your video to match.", GSBold);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Your videos are in : " + sp_VideoFrameRates.stringValue, GSBold);

                    

                }
#endif

                EditorGUILayout.EndHorizontal();

                GSBold.normal.textColor = Color.red;
                GSBold.fontStyle = FontStyle.Bold;
                EditorGUILayout.BeginHorizontal();

                if (sp_ProgressData.stringValue == "")
                {
                    if (sp_IsArmed.boolValue == false)
                    {
                        EditorGUILayout.LabelField("Previewing", GSBold);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Recording Frame " + sp_TheFrame.intValue.ToString(), GSBold);
                    }
                }
                else
                {
                    if (sp_IsArmed.boolValue == false)
                    {
                        EditorGUILayout.LabelField("Previewing", GSBold);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Recording Frame " + sp_ProgressData.stringValue, GSBold);
                    }
                }




                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("");
            }

            switch (UtopiaWorx.Helios.Helios2D.Helios2DStatus)
            {
                case 0:

                    break;
                case 1:
                    EditorGUILayout.LabelField("Helios is Compiling Your Video, this could take a while", GSBold);

                    break;
                case 2:
                    EditorGUILayout.LabelField("Helios is Adding your 3D metadata", GSBold);

                    break;

            }
            EditorGUILayout.BeginHorizontal(gsTestHeader);
            if (UnityEditor.EditorApplication.isPlaying == false)
            {
                if (MyTarget.gameObject.GetComponent<Helios.HeliosSessionRecorder>() != null)
                {
                    if (MyTarget.gameObject.GetComponent<Helios.HeliosSessionRecorder>().SaveSession == true)
                    {
                        EditorGUILayout.HelpBox("Camera Can't be armed while session recorder is set to 'Save Session'", MessageType.Info);
                    }
                    else
                    {
                        sp_IsArmed.boolValue = EditorGUILayout.Toggle("Camera Armed", sp_IsArmed.boolValue);
                    }
                }
                else
                {
                    sp_IsArmed.boolValue = EditorGUILayout.Toggle("Camera Armed", sp_IsArmed.boolValue);
                }


            }


            EditorGUILayout.EndHorizontal();

            gsTestHeader.normal.background = MakeTex(400, 1, new Color(0.0f / 255.0f, 0.0f / 255.0f, 128.0f / 255.0f, 0.25f));

            EditorGUILayout.LabelField("");

            EditorGUILayout.BeginHorizontal(gsTestHeader);

            sp_FoldoutGeneral.boolValue = EditorGUILayout.Foldout(sp_FoldoutGeneral.boolValue, "General Settings");
            EditorGUILayout.EndHorizontal();
            if (sp_FoldoutGeneral.boolValue)
            {

                if (sp_CinematicAspects.boolValue == true)
                {
                    sp_Quality.intValue = EditorGUILayout.Popup("Quality", sp_Quality.intValue, QualitySettings);
                    if (sp_Quality.intValue > 2 && sp_Quality.intValue < 5 && SystemInfo.graphicsMemorySize < 1025)
                    {
                        EditorGUILayout.HelpBox("Warning, the format you have selected may be too big for your video memory and could cause unstability", MessageType.Warning);
                    }
                }
                else
                {
                    sp_Quality.intValue = EditorGUILayout.Popup("Quality", sp_Quality.intValue, QualitySettingsCubic);
                    if (sp_Quality.intValue > 2 && sp_Quality.intValue < 5 && SystemInfo.graphicsMemorySize < 1025)
                    {
                        EditorGUILayout.HelpBox("Warning, the format you have selected may be too big for your video memory and could cause unstability", MessageType.Warning);
                    }
                }

                //	UnityEngine.Debug.Log(sp_Quality.intValue.ToString(), + " " + SystemInfo.graphicsMemorySize.ToString());

               // sp_AntiAliasingLevel.intValue = EditorGUILayout.Popup("Antialiasing", sp_AntiAliasingLevel.intValue, AntiAliasingLevels);
                sp_CaptureRate.intValue = EditorGUILayout.Popup("Frame Rate:", sp_CaptureRate.intValue, CaptureRates);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                sp_CinematicAspects.boolValue = EditorGUILayout.Toggle("Cinematic Aspects", sp_CinematicAspects.boolValue);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                sp_CopyFOV.boolValue = EditorGUILayout.Toggle("Replicate FOV", sp_CopyFOV.boolValue);
                EditorGUILayout.EndHorizontal();

                //EditorGUILayout.BeginHorizontal();
                //GUILayout.FlexibleSpace();
                //sp_UseEXR.boolValue = EditorGUILayout.Toggle("Use EXR",sp_UseEXR.boolValue);
                //EditorGUILayout.EndHorizontal();



                //if(sp_UseEXR.boolValue == false)
                //{
                //	EditorGUILayout.BeginHorizontal();
                //	GUILayout.FlexibleSpace();
                //	sp_JPEG.boolValue = EditorGUILayout.Toggle("JPEG",sp_JPEG.boolValue);
                //	EditorGUILayout.EndHorizontal();
                //	if(sp_JPEG.boolValue == true)
                //	{
                //		sp_JPEG_Quality.intValue = EditorGUILayout.IntSlider(new GUIContent("JPEG Quality",""),sp_JPEG_Quality.intValue,1,100);
                //	}
                //}


                EditorGUILayout.BeginHorizontal();
                sp_CapQual.intValue = EditorGUILayout.Popup("Capture Format", sp_CapQual.intValue, StillFormat);
                EditorGUILayout.EndHorizontal();

                switch (sp_CapQual.intValue)
                {
                    case 0:
                        sp_UseEXR.boolValue = true;
                        sp_JPEG.boolValue = false;
                        sp_PNG.boolValue = false;

                        break;
                    case 1:
                        sp_UseEXR.boolValue = false;
                        sp_JPEG.boolValue = true;
                        sp_PNG.boolValue = false;
                        break;
                    case 2:
                        sp_UseEXR.boolValue = false;
                        sp_JPEG.boolValue = false;
                        sp_PNG.boolValue = true;
                        break;
                }




                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                //            sp_UseEXR.boolValue = EditorGUILayout.Toggle("Use EXR",sp_UseEXR.boolValue);
                //EditorGUILayout.EndHorizontal();

                //if (sp_UseEXR.boolValue == false)
                //{
                //    EditorGUILayout.BeginHorizontal();
                //    GUILayout.FlexibleSpace();
                //    sp_JPEG.boolValue = EditorGUILayout.Toggle("JPEG", sp_JPEG.boolValue);
                //    EditorGUILayout.EndHorizontal();
                if (sp_JPEG.boolValue == true)
                {

                    sp_JPEG_Quality.intValue = EditorGUILayout.IntSlider(new GUIContent("JPEG Quality", ""), sp_JPEG_Quality.intValue, 1, 100);
                }
                //}
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("");
                sp_RecordMode.intValue = EditorGUILayout.Popup("Record Mode:", sp_RecordMode.intValue, RecordModes);
                if (sp_RecordMode.intValue == 1)
                {
                    LocalStart = (float)sp_StartFrame.intValue;
                    Localend = (float)sp_EndFrame.intValue;
                    EditorGUILayout.MinMaxSlider(new GUIContent("Frame Range", "The frames you would like to record"), ref LocalStart, ref Localend, 0.0f, 10000.0f);

                    sp_StartFrame.intValue = (int)LocalStart;
                    sp_EndFrame.intValue = (int)Localend;
                    EditorGUILayout.BeginHorizontal();
                    sp_StartFrame.intValue = EditorGUILayout.IntField(sp_StartFrame.intValue);
                    sp_EndFrame.intValue = EditorGUILayout.IntField(sp_EndFrame.intValue);
                    EditorGUILayout.EndHorizontal();
                    if (sp_EndFrame.intValue <= sp_StartFrame.intValue)
                    {
                        EditorGUILayout.HelpBox("End Frame must be after Start Frame", MessageType.Warning);
                    }
                    float FrameR = 0.0f;
                    switch (sp_CaptureRate.intValue)
                    {
                        case 0:
                            FrameR = 9.0f;
                            break;
                        case 1:
                            FrameR = 15.0f;
                            break;
                        case 2:
                            FrameR = 24.0f;
                            break;
                        case 3:
                            FrameR = 25.0f;
                            break;
                        case 4:
                            FrameR = 30.0f;
                            break;
                        case 5:
                            FrameR = 60.0f;
                            break;
                        case 6:
                            FrameR = 90.0f;
                            break;
                    }
                    float secs = ((Localend - LocalStart) / FrameR);
                    EditorGUILayout.LabelField(secs.ToString() + " seconds (estimated)");
                }
                if (sp_RecordMode.intValue == 2)
                {
                    EditorGUILayout.PropertyField(sp_HotKey);
                    //sp_HotKey.enumValueIndex = (UnityEngine.KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Hot Key","The key that will activate Helios"),(UnityEngine.KeyCode)sp_HotKey.enumValueIndex);
                }
                sp_SupportMultiScene.boolValue = EditorGUILayout.Toggle("Allow Multiscene", sp_SupportMultiScene.boolValue);
                if (FFMExists == true)
                {
                    sp_StreamToFFMPEG.boolValue = EditorGUILayout.Toggle("[Beta] Stream Direct to FFMPEG", sp_StreamToFFMPEG.boolValue);
                }
                else
                {
                    sp_StreamToFFMPEG.boolValue = false;
                }



                EditorGUILayout.LabelField("");


                EditorGUILayout.LabelField("Working Folder");
                EditorGUILayout.BeginHorizontal();


                sp_WorkingDir.stringValue = EditorGUILayout.TextField(sp_WorkingDir.stringValue);

                if (GUILayout.Button("..."))
                {
                    string TempDir = EditorUtility.OpenFolderPanel("Pick a working directory", "", "");
                    if (TempDir != "")

                    {
                        sp_WorkingDir.stringValue = TempDir;

                    }
                }
                if (GUILayout.Button("Open"))
                {
                    EditorUtility.RevealInFinder(sp_WorkingDir.stringValue + "/");
                }

                if (sp_WorkingDir.stringValue == "")
                {
                    sp_WorkingDir.stringValue = Application.dataPath + "/" + "UtopiaWorx/Helios/Project/";
                }
                if (System.IO.Directory.Exists(sp_WorkingDir.stringValue) == false)
                {
                    sp_WorkingDir.stringValue = Application.dataPath + "/" + "UtopiaWorx/Helios/Project/";
                }

                EditorGUILayout.EndHorizontal();
                if (sp_WorkingDir.stringValue.Contains(Application.dataPath))
                {
                    EditorGUILayout.HelpBox("Using a directory inside of your Unity Project will cause Unity to import thousands of images when Helios completes recording. It's better to pick a directory not in your Unity Project (SSD best)", MessageType.Warning);

                }

                sp_FileFormat.stringValue = EditorGUILayout.TextField(new GUIContent("File Name Template", "What naming template to use for your saved files."), sp_FileFormat.stringValue);
                if (sp_FileFormat.stringValue.Length == 0)
                {
                    EditorGUILayout.HelpBox("You must specify a file format template, otherwise the images will not save with a proper name, and Helios can't make a proper video file.", MessageType.Error);
                }
            }
            gsTestHeader.normal.background = MakeTex(400, 1, new Color(0.0f / 255.0f, 128.0f / 255.0f, 0.0f / 255.0f, 0.25f));

            EditorGUILayout.LabelField("");
            EditorGUILayout.BeginHorizontal(gsTestHeader);
            sp_FoldoutAdvanced.boolValue = EditorGUILayout.Foldout(sp_FoldoutAdvanced.boolValue, "Effects Settings");
            EditorGUILayout.EndHorizontal();
            if (sp_FoldoutAdvanced.boolValue)
            {
                if (sp_JPEG.boolValue == false)
                {
                    sp_UseChromaKey.boolValue = EditorGUILayout.Toggle("Chroma Key Objects", sp_UseChromaKey.boolValue);
                    if (sp_UseChromaKey.boolValue == true)
                    {


                        sp_ChromaSky.boolValue = EditorGUILayout.Toggle("Chroma Key Sky", sp_ChromaSky.boolValue);

                        EditorGUILayout.LabelField("Key Color");
                        sp_ChromaColor.colorValue = EditorGUILayout.ColorField(sp_ChromaColor.colorValue);
                        sp_ChromaBlend.floatValue = EditorGUILayout.Slider(new GUIContent("Chroma Bleed", "How relaxed to be about chroma matching."), sp_ChromaBlend.floatValue, 0.0f, 0.1f);



                    }
                }
                else
                {
                    sp_UseChromaKey.boolValue = false;
                    sp_ChromaSky.boolValue = false;
                }

                sp_GammaBoost.boolValue = EditorGUILayout.Toggle("Gamma Boost", sp_GammaBoost.boolValue);

                if (sp_GammaBoost.boolValue == true)
                {
                    sp_GammaBoostAmount.floatValue = EditorGUILayout.Slider(new GUIContent("Gamma Amount", "How much Gamma correction to add to the scene."), sp_GammaBoostAmount.floatValue, 0.001f, 0.6f);
                }
                sp_GrainStrength.floatValue = EditorGUILayout.Slider(new GUIContent("Grain Amount", "How much grain to add to the scene."), sp_GrainStrength.floatValue, 0.0f, 0.1f);

                sp_FadeInTime.floatValue = EditorGUILayout.Slider(new GUIContent("Fade In Time", "How long to fade into the scene from black"), sp_FadeInTime.floatValue, 0.0f, 2.0f);
                if (sp_RecordMode.intValue == 1)
                {
                    sp_FadeOutTime.floatValue = EditorGUILayout.Slider(new GUIContent("Fade Out Time ", "How long to fade out the scene to black."), sp_FadeOutTime.floatValue, 0.0f, 2.0f);
                }
            }

            gsTestHeader.normal.background = MakeTex(400, 1, new Color(128.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f, 0.25f));
            EditorGUILayout.LabelField("");
            EditorGUILayout.BeginHorizontal(gsTestHeader);
            sp_FoldoutOutput.boolValue = EditorGUILayout.Foldout(sp_FoldoutOutput.boolValue, "Output Settings");
            EditorGUILayout.EndHorizontal();
            if (sp_FoldoutOutput.boolValue)
            {


                sp_OutputFormat.intValue = EditorGUILayout.Popup("Output Format", sp_OutputFormat.intValue, OutputFormats);
                sp_ClipDuration.floatValue = EditorGUILayout.Slider(new GUIContent("Clip Length (Seconds)", "Even if you record X number of frames, only render this many. Good for testing."), sp_ClipDuration.floatValue, 0.001f, 99999.999f);
                sp_EncodingPreset.intValue = EditorGUILayout.Popup("Encoding Quality", sp_EncodingPreset.intValue, EncodingPresets);
                EditorGUILayout.LabelField(new GUIContent("Soundtrack File", "Pick a soundtrack file"));
                sp_Soundtrack.objectReferenceValue = EditorGUILayout.ObjectField(sp_Soundtrack.objectReferenceValue, typeof(AudioClip), true) as AudioClip;



                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                sp_DebugMode.boolValue = EditorGUILayout.Toggle("Debug Mode", sp_DebugMode.boolValue);
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                sp_DeleteSource.boolValue = EditorGUILayout.Toggle("Delete Stills after Video", sp_DeleteSource.boolValue);
                EditorGUILayout.EndHorizontal();



                if (FFMExists == true)
                {
                    sp_AdditionalSwitches.stringValue = EditorGUILayout.TextField(new GUIContent("FFMPEG Switches", "Any additional command line switches you want to sent to FFMPEG"), sp_AdditionalSwitches.stringValue);
                }









            }

            EditorGUILayout.LabelField("");


            gsTestHeader.normal.background = MakeTex(400, 1, new Color(128.0f / 255.0f, 64.0f / 255.0f, 128.0f / 255.0f, 0.25f));

            EditorGUILayout.BeginHorizontal(gsTestHeader);
            sp_FoldoutDiags.boolValue = EditorGUILayout.Foldout(sp_FoldoutDiags.boolValue, "Diagnostics");
            EditorGUILayout.EndHorizontal();
            if (sp_FoldoutDiags.boolValue)
            {
                EditorGUILayout.LabelField("Unity");
                EditorGUILayout.LabelField("Unity Version: " + Application.unityVersion.ToString());
                EditorGUILayout.LabelField("");

                EditorGUILayout.LabelField("GPU");
                EditorGUILayout.LabelField("GPU Make: " + SystemInfo.graphicsDeviceVendor.ToString());
                EditorGUILayout.LabelField("GPU Model: " + SystemInfo.graphicsDeviceName.ToString());
                EditorGUILayout.LabelField("GPU RAM: " + SystemInfo.graphicsMemorySize.ToString());
                EditorGUILayout.LabelField("Shader Level: " + SystemInfo.graphicsShaderLevel.ToString());

                EditorGUILayout.LabelField("");
                EditorGUILayout.LabelField("System");
                EditorGUILayout.LabelField("Device: " + SystemInfo.deviceName.ToString());
                EditorGUILayout.LabelField("Model: " + SystemInfo.deviceModel.ToString());

                EditorGUILayout.LabelField("RAM: " + SystemInfo.systemMemorySize.ToString());
                EditorGUILayout.LabelField("Processor(s): " + SystemInfo.processorType.ToString());
                EditorGUILayout.LabelField("Threads: " + SystemInfo.processorCount.ToString());
                EditorGUILayout.LabelField("Speed: " + SystemInfo.processorFrequency.ToString());
                EditorGUILayout.LabelField("");
                EditorGUILayout.LabelField("OS");
                EditorGUILayout.LabelField("OS Make: " + SystemInfo.operatingSystem.ToString());

            }
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("");

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (UnityEditor.EditorApplication.isPlaying == false && Helios.Helios.HeliosStatus == 0)
            {
                if (FFMExists == true)
                {
                    if (GUILayout.Button(MakeVideo, GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        string OS = "";
#if UNITY_EDITOR_WIN
                        OS = "win";
#endif

#if UNITY_EDITOR_OSX
					OS = "osx";
#endif
                        string PathDir = Application.dataPath + "/" + "UtopiaWorx/Helios/Bin/ffmpeg/" + OS + "/";
                        string AudioPath = "";
                        if (sp_Soundtrack.objectReferenceValue != null)
                        {
                            AudioClip TheClip = (AudioClip)sp_Soundtrack.objectReferenceValue as AudioClip;
#if UNITY_EDITOR_WIN
                            AudioPath = "-i \"" + Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(TheClip) + "\"";
#endif

#if UNITY_EDITOR_OSX
						AudioPath = "-i '" + Application.dataPath.Replace("Assets","") + AssetDatabase.GetAssetPath(TheClip) + "'";
#endif
                        }
                        string path = sp_WorkingDir.stringValue + "/";
                        int FRames = 30;
                        switch (sp_CaptureRate.intValue)
                        {
                            case 0:
                                FRames = 9;
                                break;
                            case 1:
                                FRames = 15;
                                break;
                            case 2:
                                FRames = 24;
                                break;
                            case 3:
                                FRames = 25;
                                break;
                            case 4:
                                FRames = 30;
                                break;
                            case 5:
                                FRames = 60;
                                break;
                            case 6:
                                FRames = 90;
                                break;

                        }
                        bool Delete = sp_DeleteSource.boolValue;
                        bool Do3D = false;
                        bool DoDebug = sp_DebugMode.boolValue;
                        float LocalDur = sp_ClipDuration.floatValue;
                        //string SceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
                        string SceneName = sp_FileFormat.stringValue;
                        if (SceneName.Length == 0)
                        {
                            SceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
                        }
                        string Scale = "";
                        if (sp_CinematicAspects.boolValue == true)
                        {
                            switch (sp_Quality.intValue)
                            {
                                case 0:
                                    Scale = "-vf scale=854:480";
                                    break;

                                case 1:
                                    Scale = "-vf scale=1280:720";
                                    break;

                                case 2:
                                    Scale = "-vf scale=1920:1080";
                                    break;

                                case 3:
                                    Scale = "-vf scale=3840:2160";
                                    break;
                            }
                        }

                        string Preset = "";
                        switch (sp_EncodingPreset.intValue)
                        {
                            case 0:
                                Preset = "-preset ultrafast";
                                break;
                            case 1:
                                Preset = "-preset superfast";
                                break;
                            case 2:
                                Preset = "-preset veryfast";
                                break;
                            case 3:
                                Preset = "-preset faster";
                                break;
                            case 4:
                                Preset = "-preset fast";
                                break;
                            case 5:
                                Preset = "-preset medium";
                                break;
                            case 6:
                                Preset = "-preset slow";
                                break;
                            case 7:
                                Preset = "-preset slower";
                                break;
                            case 8:
                                Preset = "-preset veryslow";
                                break;


                        }
                        string FileFormat = "";
                        if (sp_JPEG.boolValue == true)
                        {
                            FileFormat = "jpg";
                        }
                        else
                        {
                            FileFormat = "png";
                        }
                        string OutPutExt = "mp4";

                        switch (sp_OutputFormat.intValue)
                        {
                            case 0:
                                OutPutExt = "mp4";
                                break;
                            case 1:
                                OutPutExt = "gif";
                                break;
                            case 2:
                                OutPutExt = "avi";
                                break;
                            case 3:
                                OutPutExt = "mov";
                                break;
                            case 4:
                                OutPutExt = "webm";
                                break;
                        }
                        string TheFileFormat = sp_FileFormat.stringValue;
                        string Switches = sp_AdditionalSwitches.stringValue;
                        var thread = new Thread(delegate () { Command(path, PathDir, FRames, Delete, Do3D, SceneName, AudioPath, DoDebug, LocalDur, Scale, Preset, FileFormat, OutPutExt, TheFileFormat, Switches); });
                        thread.Start();
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("You have not yet instelled the FFMPEG unility. Video compiling will require you to install this. Click Window->Helios->Install FFMPEG", MessageType.Info);
                }

            }
            EditorGUILayout.EndHorizontal();
            if (sp_IsArmed.boolValue == true)
            {
                Camera TheCam;
                TheCam = (Camera)sp_FrontCam.objectReferenceValue as Camera;
                if (TheCam != null)
                {
                    TheCam.enabled = true;

                }


            }
            else
            {
                Camera TheCam;
                TheCam = (Camera)sp_FrontCam.objectReferenceValue as Camera;
                if (TheCam != null)
                {
                    TheCam.enabled = false;
                }


            }



            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

        }

        static void Command(string input, string RunPath, int CaptureRate, bool DeleteStills, bool Do3D, string SceneName, string AudioPath, bool DebugMode, float Duration, string Scale, string Preset, string FileFormat, string OutputExt, string TemplateName, string AdditionalSwitches)
        {

            string OutputOverwrite = "";
            OutputOverwrite = OutputExt;
            if (OutputOverwrite == "gif")
            {
                OutputOverwrite = "mp4";
            }
            string Encoder = "";
            switch (OutputOverwrite)
            {
                case "mp4":
                    Encoder = "libx264";
                    break;
                case "avi":
                    Encoder = "libxvid";
                    break;
                case "mov":
                    Encoder = "libxvid";
                    break;
                case "webm":
                    Encoder = "libvpx";
                    break;
            }

            if (AdditionalSwitches.Length < 0)
            {
                AdditionalSwitches = " " + AdditionalSwitches;
            }
            System.IO.File.Delete(input + TemplateName + "0000.png");
#if UNITY_EDITOR_OSX
			string args = "-framerate " + CaptureRate.ToString() + " -y -i '" + input + TemplateName + "%05d." + FileFormat + "' " + AudioPath + " -c:v " + Encoder + " -r " + CaptureRate.ToString() + " -pix_fmt yuv420p " + Scale + " " + Preset + " -t " + Duration.ToString() +  AdditionalSwitches + " '" + input + SceneName.Replace(" ","") + "." + OutputOverwrite + "'";
			if(DebugMode == true)
			{
				UnityEngine.Debug.Log(args);
			}
			var processInfo = new ProcessStartInfo( RunPath + "ffmpeg",args);
			UtopiaWorx.Helios.Helios2D.Helios2DStatus = 0;
			processInfo.CreateNoWindow = true;
			UtopiaWorx.Helios.Helios2D.Helios2DStatus = 1;
			var process = Process.Start(processInfo);
			process.WaitForExit();
			process.Close();

			if(OutputExt == "gif")
			{
				string argsGif1 =  "-y -i '" + input + SceneName.Replace(" ","") + "." + "mp4" +"'"  +  " -vf format=rgb24,fps=" + CaptureRate.ToString() + "," + Scale.Replace("-vf ","") + ":flags=lanczos,palettegen '" + input + "palette.png'";

				if(DebugMode == true)
				{
					UnityEngine.Debug.Log(argsGif1);
				}
				var processInfo3 = new ProcessStartInfo( RunPath + "ffmpeg",argsGif1);
				processInfo3.CreateNoWindow = false;
				var process3 = Process.Start(processInfo3);
				process3.WaitForExit();
				process3.Close();


				string argsGif2 = "-y -i '" + input + SceneName.Replace(" ","") + "." + "mp4" +"'" + " -i '" + input + "palette.png' -filter_complex 'fps=" + CaptureRate.ToString() + "," + Scale.Replace("-vf ","") + ":flags=lanczos[x];[x][1:v]paletteuse'  -r 20 '" + input + SceneName.Replace(" ","") + ".gif'";
				if(DebugMode == true)
				{
					UnityEngine.Debug.Log(argsGif2);
				}

				var processInfo4 = new ProcessStartInfo( RunPath + "ffmpeg",argsGif2);
				processInfo4.CreateNoWindow = false;
				var process4 = Process.Start(processInfo4);
				process4.WaitForExit();
				process4.Close();
				//					./ffmpeg  -i '/HeliosTemp/3D_Demo.mp4' -i '/HeliosTemp/palette.png' -filter_complex "fps=60,scale=854:-1:flags=lanczos[x];[x][1:v]paletteuse"  -r 20 '/HeliosTemp/3D_Demo.gif'


			}

			if(DeleteStills == true)
			{
				foreach(string MyFile in System.IO.Directory.GetFiles(input,"*." + FileFormat))
				{
					System.IO.File.Delete(MyFile);
				}
				//System.IO.File.Delete(input + SceneName +".mp4");
			}

#endif

#if UNITY_EDITOR_WIN


            string args = "-framerate " + CaptureRate.ToString() + " -y -i \"" + input.Replace(@"/", @"\") + TemplateName + "%05d." + FileFormat + "\" " + AudioPath.Replace(@"/", @"\") + " -c:v " + Encoder + " -r " + CaptureRate.ToString() + " -pix_fmt yuv420p " + Scale + " " + Preset + " -t " + Duration.ToString() + AdditionalSwitches + " \"" + input.Replace(@"/", @"\") + SceneName + "." + OutputOverwrite + "\"";
            if (DebugMode == true)
            {
                UnityEngine.Debug.Log(args);
            }
            var processInfo = new ProcessStartInfo(RunPath + "ffmpeg.exe", args);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            UtopiaWorx.Helios.Helios2D.Helios2DStatus = 1;
            var process = Process.Start(processInfo);
            process.WaitForExit();
            process.Close();
            UtopiaWorx.Helios.Helios2D.Helios2DStatus = 0;

            if (OutputExt == "gif")
            {
                string argsGif1 = "-y -i \"" + input.Replace(@"/", @"\") + SceneName.Replace(" ", "") + "." + "mp4" + "\"" + " -vf format=rgb24,fps=" + CaptureRate.ToString() + "," + Scale.Replace("-vf ", "") + ":flags=lanczos,palettegen \"" + input.Replace(@"/", @"\") + "palette.png\"";

                if (DebugMode == true)
                {
                    UnityEngine.Debug.Log(argsGif1);
                }
                var processInfo3 = new ProcessStartInfo(RunPath + "ffmpeg", argsGif1);
                processInfo3.CreateNoWindow = false;
                var process3 = Process.Start(processInfo3);
                process3.WaitForExit();
                process3.Close();


                string argsGif2 = "-y -i \"" + input.Replace(@"/", @"\") + SceneName.Replace(" ", "") + "." + "mp4" + "\"" + " -i \"" + input.Replace(@"/", @"\") + "palette.png\" -filter_complex \"fps=" + CaptureRate.ToString() + "," + Scale.Replace("-vf ", "") + ":flags=lanczos[x];[x][1:v]paletteuse\"  -r 20 \"" + input.Replace(@"/", @"\") + SceneName.Replace(" ", "") + ".gif\"";
                if (DebugMode == true)
                {
                    UnityEngine.Debug.Log(argsGif2);
                }

                var processInfo4 = new ProcessStartInfo(RunPath + "ffmpeg", argsGif2);
                processInfo4.CreateNoWindow = false;
                var process4 = Process.Start(processInfo4);
                process4.WaitForExit();
                process4.Close();



            }

            if (DeleteStills == true)
            {
                foreach (string MyFile in System.IO.Directory.GetFiles(input, "*." + FileFormat))
                {
                    System.IO.File.Delete(MyFile);
                }
                //System.IO.File.Delete(input + SceneName +".mp4");
            }

#endif

        }
    }
}
