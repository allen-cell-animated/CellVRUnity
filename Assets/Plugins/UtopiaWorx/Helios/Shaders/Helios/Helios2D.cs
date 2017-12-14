/*
contact john@smarterphonelabs.com
*/
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Diagnostics;
using UtopiaWorx;
using UtopiaWorx.Helios;
using UtopiaWorx.Helios.Effects;
using FFmpegOut;

namespace UtopiaWorx.Helios
{

    //	#if UNITY_EDITOR
    //	[ExecuteInEditMode]
    //	#endif
    [HelpURL("http://www.utopiaworx.com/Helios/Helios2D.aspx")]
    [RequireComponent(typeof(UnityEngine.Camera))]
    [RequireComponent(typeof(UnityEngine.AudioListener))]
    public class Helios2D : MonoBehaviour
    {
#if UNITY_2017_1_OR_NEWER
        public bool FrameRateWarning = false;
        public string VideoFrameRates = "";
        public UnityEngine.Video.VideoPlayer[] VPlayers;
#endif
        public string AdditionalSwitches;
        public int CapQual = 1;
        public bool StreamToFFMPEG = false;
        public bool UseEXR = true;
        public bool UsePNG = false;
        public bool FoldoutDiags = false;
        public float ChromaBlend = 0.001f;
        public bool UseChromaKey = false;
        public bool ChromaSky;
        public Color ChromaColor;
        public float GrainStrength = 0.01f;
        public bool FoldoutAdvanced = true;
        public bool SupportMultiScene = false;
        public UtopiaWorx.Helios.Effects.FadeBlack Fader;
        public float FadeInTime = 1.0f;
        public float FadeOutTime = 1.0f;
        public string FileFormat = "img";
        private int ActualFrame = 0;
        public int RecordMode = 0;
        public int StartFrame = 0;
        public int EndFrame = 0;
        [SerializeField]
        public KeyCode HotKey;
        public bool HotKeyEnabled = false;
        public bool RemoteActive = false;
        public bool GammaBoost = true;
        public float GammaAmount = 0.0f;
        public int EncodingPreset = 0;
        public int AntiAliasingLevel = 0;
        public static int Helios2DStatus = 0;
        public bool CinematicAspects = false;
        public bool JPEG = false;
        public int JPEG_Quality = 100;
        public bool FoldoutGeneral = true;
        public bool FoldoutGraphics = true;
        public bool FoldoutOutput = true;
        public bool DeleteSource = false;
        public bool IsArmed = true;
        public bool Process3D = true;
        public bool DebugMode = false;
        public AudioClip Soundtrack;
        public float ClipDuration = 99999.0f;
        public bool CopyFOV = false;
        public string ProgressData = "";
        public int TheFrame = 0;
        public string WorkingDir = "";
        public int OutputFormat = 0;
        private bool OverwriteChecked = false;
        public RenderTexture RTForward;
        public Camera FrontCam;
        public Camera ThisCam;
        private byte[] TheBytes;
        public int Quality = 0;
        private int CachedQulity = -1;
        public int CaptureRate = 60;
        public Texture2D SaveFile;
        public Shader _Shader;


        #region FFMPEG STREAM
        FFmpegPipe _pipe;
        #endregion

        public Shader TheShader
        {
            get
            {
                if (_Shader == null)
                    _Shader = Shader.Find(ShaderName());

                return _Shader;
            }
        }
        protected Material _Material;
        public Material SourceMaterial
        {
            get
            {
                if (_Material == null)
                {
                    _Material = new Material(TheShader);
                    _Material.hideFlags = HideFlags.HideAndDontSave;
                }

                return _Material;
            }
        }
        protected virtual void OnDisable()
        {
            if (_Material)
            {
                DestroyImmediate(_Material);
            }
            _Material = null;
        }

        void OnAudioFilterRead(float[] data, int channels)
        {

        }

        void FixedUpdate()
        {


            //if (CopyFOV == true && FrontCam != null && ThisCam != null)
            //{
            //    FrontCam.fieldOfView = ThisCam.fieldOfView;
            //}

            if (OverwriteChecked == false && IsArmed == true)
            {

                OverwriteChecked = true;
#if UNITY_EDITOR
                string FT = "";
                if (JPEG == true)
                {
                    FT = "jpg";
                }
                if (UsePNG == true)
                {
                    FT = "png";
                }
                if (System.IO.Directory.GetFiles(WorkingDir, "*." + FT).Length > 0)
                {
                    bool Overwrite = EditorUtility.DisplayDialog("Overwrite Files", "It seems that there are already files in this folder, are you sure you want to overwrite them", "Yes, Overwrite!", "No, Abort Scene");

                    if (Overwrite == false)
                    {
                        UnityEditor.EditorApplication.isPlaying = false;
                        IsArmed = false;
                    }
                }
#endif
            }

            if (Input.GetKeyUp(HotKey) == true)
            {
                if (HotKeyEnabled == true)
                {
                    HotKeyEnabled = false;
                    UnityEngine.Debug.Log("Change Key State");
                }
                else
                {

                    HotKeyEnabled = true;
                    UnityEngine.Debug.Log("Change Key State");
                }
            }
            //if (FrontCam == null)
            //{
            //    Camera[] CamList = gameObject.GetComponentsInChildren<Camera>();
            //    foreach (Camera MyCam in CamList)
            //    {
            //        if (MyCam.gameObject.name == "Front")
            //        {
            //            FrontCam = MyCam;
            //        }

            //    }
            //}

            if (CachedQulity != Quality)
            {
                #region Re-align cameras
                //FrontCam.transform.localPosition = new Vector3(0,0,0);
                //FrontCam.transform.eulerAngles = new Vector3(0,0,0);


                #endregion

                string OS = "";

#if UNITY_EDITOR_WIN
                OS = "Windows";
#endif

#if UNITY_EDITOR_OSX
				OS = "OSX";
#endif

                string Aspcts = "Cubic";
                if (CinematicAspects == true)
                {
                    Aspcts = "Cinematic";
                }
                else
                {
                    Aspcts = "Cubic";
                }
                switch (Quality)
                {
                    case 0:
                        FrontCam.targetTexture = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + "RenderTextures/2D/" + OS + "/" + Aspcts + "/Low/Front.renderTexture", typeof(RenderTexture));
                        RTForward = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + "RenderTextures/2D/" + OS + "/" + Aspcts + "/Low/Front.renderTexture", typeof(RenderTexture));
                        break;

                    case 1:
                        FrontCam.targetTexture = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + "RenderTextures/2D/" + OS + "/" + Aspcts + "/Medium/Front.renderTexture", typeof(RenderTexture));
                        RTForward = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + "RenderTextures/2D/" + OS + "/" + Aspcts + "/Medium/Front.renderTexture", typeof(RenderTexture));
                        break;

                    case 2:
                        FrontCam.targetTexture = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + "RenderTextures/2D/" + OS + "/" + Aspcts + "/High/Front.renderTexture", typeof(RenderTexture));
                        RTForward = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + "RenderTextures/2D/" + OS + "/" + Aspcts + "/High/Front.renderTexture", typeof(RenderTexture));
                        break;

                    case 3:

                        FrontCam.targetTexture = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + "RenderTextures/2D/" + OS + "/" + Aspcts + "/Extreme/Front.renderTexture", typeof(RenderTexture));
                        RTForward = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + "RenderTextures/2D/" + OS + "/" + Aspcts + "/Extreme/Front.renderTexture", typeof(RenderTexture));
                        break;

                    //Helios 1.0.6
                    case 4:

                        FrontCam.targetTexture = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + "RenderTextures/2D/" + OS + "/" + Aspcts + "/8K/Front.renderTexture", typeof(RenderTexture));
                        RTForward = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + "RenderTextures/2D/" + OS + "/" + Aspcts + "/8K/Front.renderTexture", typeof(RenderTexture));
                        break;
                    //Helios 1.0.6

                    //Helios 1.0.6
                    case 5:

                        FrontCam.targetTexture = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + "RenderTextures/2D/" + OS + "/" + Aspcts + "/1400/Front.renderTexture", typeof(RenderTexture));
                        RTForward = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + "RenderTextures/2D/" + OS + "/" + Aspcts + "/1400/Front.renderTexture", typeof(RenderTexture));
                        break;
                    case 6:

                        FrontCam.targetTexture = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + "RenderTextures/2D/" + OS + "/" + Aspcts + "/iPad/Front.renderTexture", typeof(RenderTexture));
                        RTForward = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + "RenderTextures/2D/" + OS + "/" + Aspcts + "/iPad/Front.renderTexture", typeof(RenderTexture));
                        break;
                    case 7:

                        FrontCam.targetTexture = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + "RenderTextures/2D/" + OS + "/" + Aspcts + "/iPhone5/Front.renderTexture", typeof(RenderTexture));
                        RTForward = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + "RenderTextures/2D/" + OS + "/" + Aspcts + "/iPhone5/Front.renderTexture", typeof(RenderTexture));
                        break;
                    case 8:

                        FrontCam.targetTexture = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + "RenderTextures/2D/" + OS + "/" + Aspcts + "/iPhone6/Front.renderTexture", typeof(RenderTexture));
                        RTForward = (RenderTexture)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + "RenderTextures/2D/" + OS + "/" + Aspcts + "/iPhone6/Front.renderTexture", typeof(RenderTexture));
                        break;

                        //Helios 1.0.6


                }

                if (StreamToFFMPEG == true)
                {
                    OpenPipe();
                }
                int Alias = 1;
                //switch (AntiAliasingLevel)
                //{
                //    case 0:
                //        Alias = 1;
                //        break;
                //    case 1:
                //        Alias = 2;
                //        break;
                //    case 2:
                //        Alias = 4;
                //        break;
                //    case 3:
                //        Alias = 8;
                //        break;
                //}
                //FrontCam.targetTexture.antiAliasing = Alias;
                CachedQulity = Quality;
            }


        }
        void Start()
        {
#if UNITY_2017_1_OR_NEWER

            VPlayers = GameObject.FindObjectsOfType<UnityEngine.Video.VideoPlayer>();

            if (VPlayers != null)
            {
                foreach (UnityEngine.Video.VideoPlayer VP in VPlayers)
                {
                    VideoFrameRates += VP.frameRate.ToString() + " PFS,";
                    if (VP.frameRate != CaptureRate)
                    {
                        FrameRateWarning = true;
                    }
                    VP.Prepare();
                }
            }
#endif

            try
            {
                ThisCam = GetComponent<Camera>();
                FrontCam = ThisCam;
            }
            catch
            {

            }

            if (UseChromaKey == true && ChromaSky == true)
            {
                FrontCam.clearFlags = CameraClearFlags.SolidColor;
                FrontCam.backgroundColor = ChromaColor;
            }
            else
            {
                FrontCam.clearFlags = CameraClearFlags.Skybox;
            }
            if (SupportMultiScene == true)
            {
                DontDestroyOnLoad(gameObject);
            }

            ActualFrame = 0;
            // Set the playback framerate (real time will not relate to game time after this).
#if UNITY_EDITOR

            try
            {
                Fader = GetComponentInChildren<UtopiaWorx.Helios.Effects.FadeBlack>();
                if (FadeInTime == 0.0f)
                {
                    Fader.Amount = 1.0f;
                }
                else
                {
                    Fader.Amount = 0.0f;
                }
            }
            catch
            {
                Fader = null;
            }



            if (EditorPrefs.GetBool("HeliosMax") == false)
            {
                EditorUtility.DisplayDialog("Helios", "Since this is your first time to use Helios, we suggest that you click the 'Maximize On Play' option on the game view. This will give you a much faster render time in the future. Also, if your computer has power setting that will put it to sleep or launch a screen saver, you may want to disable those while Helios is recording", "Got It!");
                EditorPrefs.SetBool("HeliosMax", true);
            }
            if (IsArmed == true)
            {
                switch (CaptureRate)
                {
                    case 0:
                        Time.captureFramerate = 9;
                        break;
                    case 1:
                        Time.captureFramerate = 15;
                        break;
                    case 2:
                        Time.captureFramerate = 24;
                        break;
                    case 3:
                        Time.captureFramerate = 25;
                        break;
                    case 4:
                        Time.captureFramerate = 30;
                        break;
                    case 5:
                        Time.captureFramerate = 60;
                        break;
                    case 6:
                        Time.captureFramerate = 90;
                        break;
                }
                Application.runInBackground = true;
            }
#endif

            try
            {
                gameObject.GetComponent<Camera>().enabled = true;
            }
            catch
            {
                UnityEngine.Debug.LogError("Helios2D requires that a camera be installed and enabled on the top level Helios2D GameObject.", this);
            }




            TheFrame = 0;



        }
        void OnApplicationQuit()
        {
            if (StreamToFFMPEG == true)
            {
                ClosePipe();
            }
        }

        protected string ShaderName()
        {
            return "Utopiaworx/Shaders/PostProcessing/Helios2D";
        }
        private string OurTempSquareImageLocation(int Frame)
        {
            //Frame = TheFrame;
            string LeadingZeros = "";
            if (Frame < 10)
            {
                LeadingZeros = "0000";
            }
            else
            {
                if (Frame < 100)
                {
                    LeadingZeros = "000";
                }
                else
                {
                    if (Frame < 1000)
                    {
                        LeadingZeros = "00";
                    }
                    else
                    {
                        if (Frame < 10000)
                        {
                            LeadingZeros = "0";
                        }
                        else
                        {
                            if (Frame < 100000)
                            {
                                LeadingZeros = "";
                            }
                        }
                    }
                }
            }


            //string PathDir = Application.dataPath + "/" + "UtopiaWorx/Shaders/Bin/ffmpeg/osx/";
            string PathDir = WorkingDir + "/";
            string r = "";
            if (UseEXR == true)
            {
                r = PathDir + FileFormat + LeadingZeros + Frame.ToString() + ".exr";

            }
            else
            {
                if (JPEG == true)
                {
                    r = PathDir + FileFormat + LeadingZeros + Frame.ToString() + ".jpg";
                }
                if (UsePNG == true)
                {
                    r = PathDir + FileFormat + LeadingZeros + Frame.ToString() + ".png";
                }
            }

            //TheFrame++;
            return r;
        }

        //		static void CommandEXR (string input, int Width, int Height,  Color[] TheBytes)
        //		{
        //			UtopiaWorx.Helios.MiniEXR.MiniEXRWrite(input, (uint)Width, (uint)Height, TheBytes);
        //
        //		}
        static void Command(string input, byte[] TheBytes)
        {
            System.IO.File.WriteAllBytes(input, TheBytes);
        }

        static void CommandStream(byte[] TheBytes, FFmpegPipe _pipe)
        {
            _pipe.Write(TheBytes);
        }

        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
#if UNITY_2017_1_OR_NEWER
            if (VPlayers != null)
            {
                foreach (UnityEngine.Video.VideoPlayer VP in VPlayers)
                {
                    VP.frame = ActualFrame;
                    VP.Play();
                }
            }
#endif

            float FrameR = 0.0f;
            switch (CaptureRate)
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

            switch (RecordMode)
            {
                case 0:
                    if (Fader != null && Fader.Amount < 1.0f)
                    {
                        Fader.Amount = Mathf.Clamp(Fader.Amount += ((FadeInTime / (FrameR * FadeInTime)) / FadeInTime), 0.0f, 1.0f);
                    }
                    break;

                case 1:
                    if ((TheFrame * 1.0f) < (FrameR * FadeInTime))
                    {
                        if (Fader != null && Fader.Amount < 1.0f)
                        {
                            Fader.Amount = Mathf.Clamp(Fader.Amount += ((FadeInTime / (FrameR * FadeInTime)) / FadeInTime), 0.0f, 1.0f);
                        }
                    }

                    if ((TheFrame * 1.0f) > ((EndFrame * 1.0f) - (FrameR * FadeOutTime)) - 1.0f)
                    {
                        if (Fader != null && Fader.Amount > 0.0f)
                        {
                            Fader.Amount = Mathf.Clamp(Fader.Amount -= ((FadeOutTime / (FrameR * FadeOutTime)) / FadeOutTime), 0.0f, 1.0f);
                        }
                    }

                    if (TheFrame > EndFrame)
                    {
                        UnityEditor.EditorApplication.isPlaying = false;
                    }
                    break;

            }

            if (SourceMaterial == null || IsArmed == false)
            {
                Graphics.Blit(source, destination);
            }
            else
            {
                bool CanRecord = false;
                switch (RecordMode)
                {
                    case 0:
                        CanRecord = true;
                        break;
                    case 1:
                        if (TheFrame >= StartFrame && TheFrame <= EndFrame)
                        {
                            CanRecord = true;
                        }
                        break;
                    case 2:
                        if (HotKeyEnabled == true)
                        {
                            CanRecord = true;
                        }
                        break;
                    case 3:
                        if (RemoteActive == true)
                        {
                            CanRecord = true;
                        }
                        break;
                    //Helios 1.0.6
                    case 4:
                        if (TheFrame < 1)
                        {
                            CanRecord = true;
                        }
                        else
                        {
                            UnityEditor.EditorApplication.isPlaying = false;
                            string Ext = "";
                            if (JPEG == true)
                            {
                                Ext = "jpg";
                            }
                            if (UsePNG == true)
                            {
                                Ext = "png";
                            }

                            EditorUtility.RevealInFinder(WorkingDir + "/" + FileFormat + "00000." + Ext);
                        }
                        break;
                        //Helios 1.0.6
                }
                if (CanRecord == true)
                {
                    if (GammaBoost == true)
                    {
//                        SourceMaterial.SetFloat("_Gamma", 0.454545f);
                        SourceMaterial.SetFloat("_Gamma", GammaAmount);
                        SourceMaterial.SetFloat("_Grain", GrainStrength);
                        SourceMaterial.SetFloat("_Seed", Random.Range(0.1f, 999.999f));



                        RenderTexture RTTemp = RenderTexture.GetTemporary(RTForward.width, RTForward.height, RTForward.depth, RTForward.format);
                        Graphics.Blit(RTForward, RTTemp, SourceMaterial, 0);
                        Graphics.Blit(RTTemp, RTForward);
                        RenderTexture.ReleaseTemporary(RTTemp);
                    }

                    if (UseChromaKey == true && JPEG == false)
                    {

                        SourceMaterial.SetFloat("_ChromaBlend", ChromaBlend);
                        SourceMaterial.SetColor("_ChromaColor", ChromaColor);
                        SourceMaterial.SetFloat("_UseChroma", 1.0f);
                    }
                    else
                    {
                        SourceMaterial.SetFloat("_UseChroma", 0.0f);
                        SourceMaterial.SetColor("_ChromaColor", Color.clear);
                    }

                    RenderTexture RTGrain = RenderTexture.GetTemporary(RTForward.width, RTForward.height, RTForward.depth, RTForward.format);
                    Graphics.Blit(RTForward, RTGrain, SourceMaterial, 1);
                    Graphics.Blit(RTGrain, RTForward);
                    RenderTexture.ReleaseTemporary(RTGrain);

                    //#if UNITY_EDITOR_OSX 
                    if (UseEXR == true)
                    {
                        RenderTexture RTFlip = RenderTexture.GetTemporary(RTForward.width, RTForward.height, RTForward.depth, RTForward.format);
                        Graphics.Blit(RTForward, RTFlip, SourceMaterial, 2);
                        Graphics.Blit(RTFlip, RTForward);
                        RenderTexture.ReleaseTemporary(RTFlip);
                    }

                    if (StreamToFFMPEG == true)
                    {
                        RenderTexture RTFlip = RenderTexture.GetTemporary(RTForward.width, RTForward.height, RTForward.depth, RTForward.format);
                        Graphics.Blit(RTForward, RTFlip, SourceMaterial, 2);
                        Graphics.Blit(RTFlip, RTForward);
                        RenderTexture.ReleaseTemporary(RTFlip);
                    }
                    //#endif

                    SaveFile = new Texture2D(RTForward.width, RTForward.height, TextureFormat.ARGB32, false, GammaBoost);
                    RenderTexture.active = RTForward;
                    if (Application.isPlaying == true)
                    {
                        SaveFile.ReadPixels(new Rect(0, 0, RTForward.width, RTForward.height), 0, 0);
                        if (StreamToFFMPEG == false)
                        {
                            string FileName = OurTempSquareImageLocation(ActualFrame);

                            if (UseEXR == true)
                            {
                                UtopiaWorx.Helios.MiniEXR.MiniEXRWrite(FileName, (uint)RTForward.width, (uint)RTForward.height, SaveFile.GetPixels());

                            }
                            else
                            {
                                if (JPEG == true)
                                {
                                    TheBytes = SaveFile.EncodeToJPG(JPEG_Quality);
                                }
                                if (UsePNG == true)
                                {
                                    TheBytes = SaveFile.EncodeToPNG();
                                }
                                var thread = new Thread(delegate () { Command(FileName, TheBytes); });
                                thread.Start();
                            }
                        }
                        else
                        {
                            //byte[] bytes = SaveFile.GetRawTextureData();
                            //var threadStream = new Thread(delegate () { CommandStream(bytes, _pipe); });
                            //threadStream.Start();
                            _pipe.Write( SaveFile.GetRawTextureData());
                        }





                    }
                    Destroy(SaveFile);


                    ActualFrame++;
                }
                Graphics.Blit(source, destination);
            }
            TheFrame++;
#if UNITY_2017_1_OR_NEWER
            if (VPlayers != null)
            {
                foreach (UnityEngine.Video.VideoPlayer VP in VPlayers)
                {

                    VP.Pause();
                }
            }
#endif        
        }



        #region FFMPEG STREAM
        void OpenPipe()
        {
            if (_pipe != null) return;


            name = WorkingDir +"/" +name;
            UnityEngine.Debug.Log(name);
            // Open an output stream.
            _pipe = new FFmpegPipe(name, RTForward.width, RTForward.height, 30, "argb");

        }

        void ClosePipe()
        {

            _pipe.Close();

        }
        #endregion

    }
}
#endif
