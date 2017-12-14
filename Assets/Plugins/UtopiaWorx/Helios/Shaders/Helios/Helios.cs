
/*
contact john@smarterphonelabs.com
*/
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Diagnostics;
using UnityEditor;
using FFmpegOut;


namespace UtopiaWorx.Helios
{


//	#if UNITY_EDITOR
//	[ExecuteInEditMode]
//	#endif
	[HelpURL("http://www.utopiaworx.com/Helios/Helios3D.aspx")]
	[RequireComponent (typeof (UnityEngine.Camera))]
	[RequireComponent (typeof (UnityEngine.AudioListener))]
	public class Helios : MonoBehaviour 
	{
#if UNITY_2017_1_OR_NEWER
        public bool FrameRateWarning = false;
        public string VideoFrameRates = "";
        public UnityEngine.Video.VideoPlayer[] VPlayers;
#endif
        public bool StreamToFFMPEG = false;
        #region FFMPEG STREAM
        FFmpegPipe _pipe;
        #endregion
        public string AdditionalSwitches;
        public bool UseEXR = true;
        public bool UsePNG = false;
		public bool FoldoutDiags = false;
        public int CapQual = 1;
		public float ChromaBlend = 0.001f;
		public bool UseChromaKey = false;
		public bool ChromaSky = false;
		public Color ChromaColor;
		public float GrainStrength = 0.01f;
		public bool SupportMultiScene = false;
		public UtopiaWorx.Helios.Effects.FadeBlack[] Fader;
		public float FadeInTime = 1.0f;
		public float FadeOutTime = 1.0f;
		public string FileFormat = "img";
		public bool SyncRotation = false;
		private int ActualFrame = 0;
		public int RecordMode =0;
		public int StartFrame = 0;
		public int EndFrame =0;
		[SerializeField]
		public KeyCode HotKey;
		public bool HotKeyEnabled = false;
		public bool RemoteActive = false;
		public float SetreoZoom = 0.25f;
		public int ProjectionMode = 0;
		public bool GammaCorrect = false;
        public float GammaAmount = 0.0f;
        public int EdgeStitching = 0;

		public bool FlipWindows = false;
		public int EncodingPreset = 0;
		public static int HeliosStatus = 0;
		public int AntiAliasingLevel = 0;
		public bool CinematicAspects = false;
		public int OutputFormat = 0;
		public float StereoOffset = 0.03f;

//		Texture2D TheT;
//		RenderTexture OutputTexture;
//		RenderTexture RTXFlip;


		public bool JPEG = false;
		public int JPEG_Quality = 100;
		public bool FoldoutGeneral = true;
		public bool FoldoutGraphics = true;
		public bool FoldoutOutput = true;
		public bool FoldoutAdvanced = true;

		public bool DeleteSource = false;
		public bool IsArmed = true;
		public bool Process3D = true;
		public bool DebugMode = false;
		public AudioClip Soundtrack;
		public float ClipDuration =99999.0f;
		public string ProgressData ="";
		public int TheFrame = 0;
		public string WorkingDir ="";


		private byte[] TheBytes;

		public int Quality = 0;
		public int CaptureRate = 60;
		public Texture2D SaveFile;

		public Shader _Shader;
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
		private bool OverwriteChecked = false;


		protected virtual void OnDisable()
		{
			if (_Material) 
			{
				DestroyImmediate (_Material);
			}
			_Material = null;
		}

		void OnAudioFilterRead(float[] data, int channels)
		{
	
		}

		void Update()
		{
			if(OverwriteChecked == false && IsArmed == true)
			{

				if(Input.GetKeyUp(HotKey) == true)
				{
					if(HotKeyEnabled == true)
					{
						HotKeyEnabled = false;

					}
					else
					{

						HotKeyEnabled = true;

					}
				}

				OverwriteChecked = true;
#if UNITY_EDITOR
				string FT = "";
				if(JPEG == true)
				{
					FT = "jpg";
				}
                if (UsePNG == true)
                {
					FT = "png";
				}
				if(System.IO.Directory.GetFiles(WorkingDir,"*." + FT  ).Length > 0)
				{
					bool Overwrite = EditorUtility.DisplayDialog("Overwrite Files", "It seems that there are already files in this folder, are you sure you want to overwrite them","Yes, Overwrite!","No, Abort Scene");

					if(Overwrite == false)
					{
						UnityEditor.EditorApplication.isPlaying = false;
						IsArmed = false;
					}
				}
#endif
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

            if (UseChromaKey == true && ChromaSky == true)
			{
				foreach(Camera Child in GetComponentsInChildren<Camera>())
				{
					Child.clearFlags = CameraClearFlags.SolidColor;
					Child.backgroundColor = ChromaColor;
				}
			}
			else
			{
				foreach(Camera Child in GetComponentsInChildren<Camera>())
				{
					Child.clearFlags = CameraClearFlags.Skybox;
				}			
			}

			if(SupportMultiScene == true)
			{
				DontDestroyOnLoad(gameObject);
			}
//1.0.8
			try
			{
				Fader = GetComponentsInChildren<UtopiaWorx.Helios.Effects.FadeBlack>();
			}
			catch
			{
				Fader = null;
			}
//1.0.8
//Helios 1.0.6	
			if(ProjectionMode == 0)
			{					
				UtopiaWorx.HeliosStitcher[] Stitchers =  gameObject.GetComponentsInChildren<UtopiaWorx.HeliosStitcher>();
				foreach(UtopiaWorx.HeliosStitcher MyStitcher in Stitchers)
				{
					if(MyStitcher.CameraLeft == true)
					{
						MyStitcher.transform.localPosition = new Vector3(0.0f,0.0f,0.0f);
					}
					else
					{
						MyStitcher.transform.localPosition = new Vector3(0.0f,0.0f,0.0f);
						MyStitcher.transform.gameObject.SetActive(false);
					}
				}
				
			}
			if(ProjectionMode == 2)
			{
				try
				{
					UtopiaWorx.HeliosStitcher[] Stitchers =  gameObject.GetComponentsInChildren<UtopiaWorx.HeliosStitcher>();
					foreach(UtopiaWorx.HeliosStitcher MyStitcher in Stitchers)
					{
						if(MyStitcher.CameraLeft == true)
						{
							MyStitcher.transform.localPosition = new Vector3(StereoOffset * -1,0.0f,0.0f);
						}
						else
						{
							MyStitcher.transform.localPosition = new Vector3(StereoOffset,0.0f,0.0f);
						}
					}
				}
				catch
				{
					
				}
			}
//Helios 1.0.6
			ActualFrame =0;
			#if UNITY_EDITOR
			if(EditorPrefs.GetBool("HeliosMax") == false)
			{
				EditorUtility.DisplayDialog("Helios","Since this is your first time to use Helios, we suggest that you click the 'Maximize On Play' option on the game view. This will give you a much faster render time in the future. Also, if your computer has power setting that will put it to sleep or launch a screen saver, you may want to disable those while Helios is recording","Got It!");
				EditorPrefs.SetBool("HeliosMax",true);
			}
			if(IsArmed == true)
			{
				switch(CaptureRate)
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
				UnityEngine.Debug.LogError("Helios requires that a camera be installed and enabled on the top level Helios GameObject.",this);
			}



           

            if (StreamToFFMPEG == true)
            {
                OpenPipe();
            }

        }

        void OnApplicationQuit()
        {
            if (StreamToFFMPEG == true)
            {
                ClosePipe();
            }
        }
        protected  string ShaderName()
		{
			return "Utopiaworx/Shaders/PostProcessing/Helios";
		}
		private string OurTempSquareImageLocation(int Frame)
		{
			string LeadingZeros = "";
			if(Frame < 10)
			{
				LeadingZeros = "0000";
			}
			else
			{
				if(Frame < 100)
				{
					LeadingZeros = "000";
				}
				else
				{
					if(Frame < 1000)
					{
						LeadingZeros = "00";
					}
					else
					{
						if(Frame < 10000)
						{
							LeadingZeros = "0";
						}
						else
						{
							if(Frame < 100000)
							{
								LeadingZeros = "";	
							}
						}
					}
				}
			}


			string PathDir = WorkingDir + "/";
			string r = "";
			if(UseEXR== true)
			{
				r = PathDir + FileFormat + LeadingZeros + Frame.ToString() + ".exr";

			}
				else
				{
				if(JPEG == true)
				{
					r = PathDir + FileFormat + LeadingZeros + Frame.ToString() + ".jpg";
				}
                if (UsePNG  == true)
                {
					r = PathDir + FileFormat + LeadingZeros + Frame.ToString() + ".png";				
				}
			}

			return r;
		}

		static void Command (string input, byte[] TheBytes)
		{
			System.IO.File.WriteAllBytes(input, TheBytes );
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
			switch(CaptureRate)
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

			switch(RecordMode)
			{
			case 0:
				for(int i = 0; i < Fader.Length; i++)
				{
					if(Fader[i] !=null && Fader[i].Amount < 1.0f)
					{
						Fader[i].Amount = Mathf.Clamp(Fader[i].Amount += ((FadeInTime / (FrameR * FadeInTime)) / FadeInTime),0.0f,1.0f);
					}
				}
				break;

			case 1:
				for(int i = 0; i < Fader.Length; i++)
				{
					if((TheFrame * 1.0f) < (FrameR * FadeInTime))
					{
						if(Fader[i] !=null && Fader[i].Amount < 1.0f)
						{
							Fader[i].Amount = Mathf.Clamp(Fader[i].Amount += ((FadeInTime / (FrameR * FadeInTime)) / FadeInTime),0.0f,1.0f);
						}
					}
				

					if((TheFrame * 1.0f) > ((EndFrame * 1.0f) - (FrameR * FadeOutTime)) -1.0f)
					{
						if(Fader[i] !=null && Fader[i].Amount > 0.0f)
						{
							Fader[i].Amount = Mathf.Clamp(Fader[i].Amount -= ((FadeOutTime / (FrameR * FadeOutTime)) / FadeOutTime),0.0f,1.0f);
						}
					}
                        if (TheFrame > EndFrame)
                        {
                            UnityEditor.EditorApplication.isPlaying = false;
                        }
                    }
				break;

                case 2:
                    for (int i = 0; i < Fader.Length; i++)
                    {
                        Fader[i].Amount = 1;
                    }
                    break;

            }
			if(SourceMaterial == null || IsArmed == false)
			{
				Graphics.Blit(source, destination);
			}
			else
			{
				bool CanRecord = false;
				switch(RecordMode)
				{
				case 0:
					CanRecord = true;
					break;
				case 1:
					if(TheFrame >= StartFrame && TheFrame <= EndFrame)
					{
						CanRecord = true;
					}
					break;
				case 2:
					if(HotKeyEnabled == true)
					{
						CanRecord = true;
					}
					break;
				case 3:
					if(RemoteActive == true)
					{
						CanRecord = true;
					}
					break;
//Helios 1.0.6
				case 4:
					if(TheFrame <1)
					{
						CanRecord = true;
					}
					else
					{
						UnityEditor.EditorApplication.isPlaying = false;
						string Ext = "";
						if(JPEG == true)
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
				if(CanRecord == true)
				{
					#region Setup Resources
					string CubePath = "";
					string CubePathR = "";
					int RecordResolutionX = 0;
					int RecordResolutionY = 0;

					switch(Quality)
					{
					case 0:
						CubePath = "Cubemaps/480.cubemap";
						CubePathR = "Cubemaps/480_R.cubemap";
						if(CinematicAspects == true)
						{
							RecordResolutionX = 704;
							RecordResolutionY = 480;
						}
						else
						{
							RecordResolutionX = 512;
							RecordResolutionY = 256;							
						}
						break;
					case 1:
						CubePath = "Cubemaps/720.cubemap";
						CubePathR = "Cubemaps/720_R.cubemap";
						if(CinematicAspects == true)
						{
							RecordResolutionX = 1280;
							RecordResolutionY = 720;
						}
						else
						{
							RecordResolutionX = 1024;
							RecordResolutionY = 512;
						}
						break;
					case 2:
						CubePath = "Cubemaps/1080.cubemap";
						CubePathR = "Cubemaps/1080_R.cubemap";
						if(CinematicAspects == true)
						{
							RecordResolutionX = 1920;
							RecordResolutionY = 1080;
						}
						else
						{
							RecordResolutionX = 2048;
							RecordResolutionY = 1024;
						}
						break;
					case 3:
						CubePath = "Cubemaps/4K.cubemap";
						CubePathR = "Cubemaps/4K_R.cubemap";
						if(CinematicAspects == true)
						{
							RecordResolutionX = 3840;
							RecordResolutionY = 2160;
						}
						else
						{
							RecordResolutionX = 4096;
							RecordResolutionY = 2048;
						}
						break;
//Helios 1.0.6
					case 4:
						CubePath = "Cubemaps/4K.cubemap";
						CubePathR = "Cubemaps/4K_R.cubemap";
						if(CinematicAspects == true)
						{
							RecordResolutionX = 7680;
							RecordResolutionY = 4320;
						}
						else
						{
							RecordResolutionX = 8192;
							RecordResolutionY = 4096;
						}
						break;
//Helios 1.0.6

//Helios 1.0.7
					case 5:
						CubePath = "Cubemaps/4K.cubemap";
						CubePathR = "Cubemaps/4K_R.cubemap";
						if(CinematicAspects == true)
						{
							RecordResolutionX = 2488;
							RecordResolutionY = 1400;
						}
						else
						{
							RecordResolutionX = 2048;
							RecordResolutionY = 1024;
						}
						break;
//Helios 1.0.7
					default:
						CubePath = "Cubemaps/480.cubemap";
						CubePathR = "Cubemaps/480_R.cubemap";
						if(CinematicAspects == true)
						{
							RecordResolutionX = 704;
							RecordResolutionY = 480;
						}
						else
						{
							RecordResolutionX = 512;
							RecordResolutionY = 256;
						}
						break;
					}




					Cubemap TheCM = (Cubemap)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + CubePath,typeof(Cubemap));
					#endregion

					#region Sync Rotation
					if(SyncRotation == true)
					{
						//Quaternion[] rot = GetComponentsInChildren<Camera>();//.transform.rotation;
						UnityEngine.Camera[] Cams = GetComponentsInChildren<Camera>();
// Helios 1.0.6
						foreach(Camera MyCamera in Cams)
						{
							if(MyCamera.transform.gameObject.name == "StitcherL")
							{

								Quaternion rot = MyCamera.transform.rotation;
								Matrix4x4 m = new Matrix4x4 ();
								m.SetTRS(Vector3.zero, rot,new Vector3(1,1,1) );
								SourceMaterial.SetMatrix("_Rotation",m);
							}

							if(MyCamera.transform.gameObject.name == "StitcherR")
							{

								Quaternion rot = MyCamera.transform.rotation;
								Matrix4x4 m = new Matrix4x4 ();
								m.SetTRS(Vector3.zero, rot,new Vector3(1,1,1) );
								SourceMaterial.SetMatrix("_Rotation",m);
							}
						}
// Helios 1.0.6
					}
					#endregion

					#region Left Eye / Mono
					SourceMaterial.SetTexture("_Cube",TheCM);

					SourceMaterial.SetFloat("_Grain",GrainStrength);
					SourceMaterial.SetFloat("_Seed",Random.Range(0.1f,999.999f));

					if(UseChromaKey == true && JPEG == false)
					{
						SourceMaterial.SetFloat("_ChromaBlend",ChromaBlend);
						SourceMaterial.SetColor("_ChromaColor",ChromaColor);
						SourceMaterial.SetFloat("_UseChroma",1.0f);
					}
					else
					{
						SourceMaterial.SetFloat("_UseChroma",0.0f);
						SourceMaterial.SetColor("_ChromaColor",Color.clear);
					}

					if(GammaCorrect == true)
					{
                        //SourceMaterial.SetFloat("_Gamma", 0.454545f);
                        SourceMaterial.SetFloat("_Gamma", GammaAmount);
                        
                        }
					else
					{
						SourceMaterial.SetFloat("_Gamma", 1.0f);
					}
					RenderTexture RTTemp = RenderTexture.GetTemporary(RecordResolutionX,RecordResolutionY,0,RenderTextureFormat.ARGB32);
				switch (ProjectionMode)
				{
					case 0:
						Graphics.Blit(source,RTTemp,SourceMaterial,0);
						break;
					case 1:
						SourceMaterial.SetFloat("_Streo_Zoom",SetreoZoom);
						Graphics.Blit(source,RTTemp,SourceMaterial,5);
						break;
//Helios 1.0.6
					case 2:
						Graphics.Blit(source,RTTemp,SourceMaterial,0);
						break;
//Helios 1.0.6

				}


					RenderTexture RTTemp2 = RenderTexture.GetTemporary(RecordResolutionX,RecordResolutionY,0,RenderTextureFormat.ARGB32);

					switch (ProjectionMode)
					{
					case 0:
						Graphics.Blit(RTTemp,RTTemp2,SourceMaterial,2);
						break;
					case 1:
						Graphics.Blit(RTTemp,RTTemp2);
						break;
//Helios 1.0.6
					case 2:
						Graphics.Blit(RTTemp,RTTemp2,SourceMaterial,2);
						break;
//Helios 1.0.6

					}



					RenderTexture.ReleaseTemporary(RTTemp);

					if(FlipWindows == true)
					{
						RenderTexture RTTemp3 = RenderTexture.GetTemporary(RecordResolutionX,RecordResolutionY,0,RenderTextureFormat.ARGB32);
						Graphics.Blit(RTTemp2,RTTemp3,SourceMaterial,1);						
						RenderTexture.ReleaseTemporary(RTTemp2);

						RTTemp2 = RenderTexture.GetTemporary(RecordResolutionX,RecordResolutionY,0,RenderTextureFormat.ARGB32);
						Graphics.Blit(RTTemp3,RTTemp2);
						RenderTexture.ReleaseTemporary(RTTemp3);
					}
					RenderTexture RTTemp4;
					if(AntiAliasingLevel > 0)
					{
						int AILvl = 1;
						switch(AntiAliasingLevel)
						{
						case 1:
							AILvl = 2;
							break;
						case 2:
							AILvl = 4;
							break;
						case 3:
							AILvl = 8;
							break;


						}
						RTTemp4 = RenderTexture.GetTemporary(RecordResolutionX,RecordResolutionY,0,RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default,AILvl);
					}
					else
					{
						RTTemp4 = RenderTexture.GetTemporary(RecordResolutionX,RecordResolutionY,0,RenderTextureFormat.ARGB32);
					}
					Graphics.Blit(RTTemp2,RTTemp4);
					RenderTexture.ReleaseTemporary(RTTemp2);

					RenderTexture RTGrain = RenderTexture.GetTemporary(RecordResolutionX, RecordResolutionY * 2,0,RenderTextureFormat.ARGB32);
					Graphics.Blit(RTTemp4,RTGrain,SourceMaterial,7);
					Graphics.Blit(RTGrain,RTTemp4);
					RenderTexture.ReleaseTemporary(RTGrain);

					#endregion

					#region Right Eye
					if(ProjectionMode == 2)
					{
//Helios 1.0.6
					










						Cubemap TheCM_R = (Cubemap)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + CubePathR,typeof(Cubemap));



						SourceMaterial.SetTexture("_Cube",TheCM_R);

						if(GammaCorrect == true)
						{
                            //SourceMaterial.SetFloat("_Gamma", 0.454545f);
                            SourceMaterial.SetFloat("_Gamma", GammaAmount);
                        }
						else
						{
							SourceMaterial.SetFloat("_Gamma", 1.0f);
						}
						RenderTexture RTTempR = RenderTexture.GetTemporary(RecordResolutionX,RecordResolutionY,0,RenderTextureFormat.ARGB32);
						switch (ProjectionMode)
						{

							//Helios 1.0.6
						case 2:
							Graphics.Blit(source,RTTempR,SourceMaterial,0);
							break;
							//Helios 1.0.6

						}


						RenderTexture RTTemp2R = RenderTexture.GetTemporary(RecordResolutionX,RecordResolutionY,0,RenderTextureFormat.ARGB32);

						switch (ProjectionMode)
						{

						case 2:
							Graphics.Blit(RTTempR,RTTemp2R,SourceMaterial,2);
							break;
							//Helios 1.0.6

						}



						RenderTexture.ReleaseTemporary(RTTempR);

						if(FlipWindows == true)
						{
							RenderTexture RTTemp3R = RenderTexture.GetTemporary(RecordResolutionX,RecordResolutionY,0,RenderTextureFormat.ARGB32);
							Graphics.Blit(RTTemp2R,RTTemp3R,SourceMaterial,1);						
							RenderTexture.ReleaseTemporary(RTTemp2R);

							RTTemp2R = RenderTexture.GetTemporary(RecordResolutionX,RecordResolutionY,0,RenderTextureFormat.ARGB32);
							Graphics.Blit(RTTemp3R,RTTemp2R);
							RenderTexture.ReleaseTemporary(RTTemp3R);
						}
						RenderTexture RTTemp4R;
						if(AntiAliasingLevel > 0)
						{
							int AILvl = 1;
							switch(AntiAliasingLevel)
							{
							case 1:
								AILvl = 2;
								break;
							case 2:
								AILvl = 4;
								break;
							case 3:
								AILvl = 8;
								break;


							}
							RTTemp4R = RenderTexture.GetTemporary(RecordResolutionX,RecordResolutionY,0,RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default,AILvl);
						}
						else
						{
							RTTemp4R = RenderTexture.GetTemporary(RecordResolutionX,RecordResolutionY,0,RenderTextureFormat.ARGB32);
						}
						Graphics.Blit(RTTemp2R,RTTemp4R);
						RenderTexture.ReleaseTemporary(RTTemp2R);



						RenderTexture RTMergeIn = RenderTexture.GetTemporary(RecordResolutionX, RecordResolutionY * 2,0,RenderTextureFormat.ARGB32);
						RenderTexture RTMerge = RenderTexture.GetTemporary(RecordResolutionX, RecordResolutionY * 2,0,RenderTextureFormat.ARGB32);

						SourceMaterial.SetTexture("_MergeL" ,RTTemp4);
						SourceMaterial.SetTexture("_MergeR" ,RTTemp4R);
						Graphics.Blit(RTMergeIn,RTMerge,SourceMaterial,6);

						RenderTexture.ReleaseTemporary(RTMergeIn);
						RenderTexture.ReleaseTemporary(RTTemp4);
						RTTemp4 = RenderTexture.GetTemporary(RecordResolutionX, RecordResolutionY * 2,0,RenderTextureFormat.ARGB32);
						Graphics.Blit(RTMerge,RTTemp4);


						RenderTexture.ReleaseTemporary(RTMerge);
						RenderTexture.ReleaseTemporary(RTTemp4R);



						RenderTexture RTGrainR = RenderTexture.GetTemporary(RecordResolutionX, RecordResolutionY * 2,0,RenderTextureFormat.ARGB32);
						Graphics.Blit(RTTemp4,RTGrainR,SourceMaterial,7);
						Graphics.Blit(RTGrainR,RTTemp4);
						RenderTexture.ReleaseTemporary(RTGrainR);
					
//Helios 1.0.6
					}
					#endregion

					#region Save File

					//#if UNITY_EDITOR_OSX 
					if(UseEXR == true)
					{
						RenderTexture RTFlip = RenderTexture.GetTemporary(RTTemp4.width, RTTemp4.height, 0,RenderTextureFormat.ARGB32);
						Graphics.Blit(RTTemp4, RTFlip,SourceMaterial,1);
						Graphics.Blit(RTFlip,RTTemp4);
						RenderTexture.ReleaseTemporary(RTFlip);
					}
					//#endif
					RenderTexture.active = RTTemp4;
					#if UNITY_EDITOR_OSX
					if(ProjectionMode == 2)
					{
						SaveFile = new Texture2D(RecordResolutionX,RecordResolutionY * 2,TextureFormat.ARGB32,false,true);
					}
					else
					{
						SaveFile = new Texture2D(RecordResolutionX,RecordResolutionY,TextureFormat.ARGB32,false,true);
					}
					#else
					if(ProjectionMode == 2)
					{
						SaveFile = new Texture2D(RecordResolutionX,RecordResolutionY * 2,TextureFormat.ARGB32, false,true);
					}
					else
					{
						SaveFile = new Texture2D(RecordResolutionX,RecordResolutionY,TextureFormat.ARGB32, false,true);
					}
					#endif

					if(ProjectionMode == 2)
					{
						SaveFile.ReadPixels(new Rect(0,0,RecordResolutionX,RecordResolutionY * 2),0,0);
					}
					else
					{
						SaveFile.ReadPixels(new Rect(0,0,RecordResolutionX,RecordResolutionY),0,0);	
					}


                    
                    if (StreamToFFMPEG == false)
                    {
                        RenderTexture.ReleaseTemporary(RTTemp4);
                        string FileName = OurTempSquareImageLocation(ActualFrame);
					if(UseEXR == true)
					{
						if(ProjectionMode == 2)
						{
							UtopiaWorx.Helios.MiniEXR.MiniEXRWrite(FileName, (uint)RecordResolutionX, (uint)RecordResolutionY * 2, SaveFile.GetPixels());
						}
						else
						{
							UtopiaWorx.Helios.MiniEXR.MiniEXRWrite(FileName, (uint)RecordResolutionX, (uint)RecordResolutionY, SaveFile.GetPixels());							
						}
					}
					else
					{
						if(JPEG == true)
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
                        //var threadStream = new Thread(delegate () { CommandStream(SaveFile.GetRawTextureData(), _pipe); });
                        //threadStream.Start();
                        _pipe.Write(SaveFile.GetRawTextureData());
                        //_pipe.Write(RTTemp4.GetNativeTexturePtr);
                        RenderTexture.ReleaseTemporary(RTTemp4);
                    }

                DestroyImmediate(SaveFile);
					#endregion

					ActualFrame++;
					
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
                Graphics.Blit(source,destination);
			}
		}


        #region FFMPEG STREAM
        void OpenPipe()
        {
            if (_pipe != null) return;



            int RecordResolutionX = 0;
            int RecordResolutionY = 0;

            switch (Quality)
            {
                case 0:

                    if (CinematicAspects == true)
                    {
                        RecordResolutionX = 704;
                        RecordResolutionY = 480;
                    }
                    else
                    {
                        RecordResolutionX = 512;
                        RecordResolutionY = 256;
                    }
                    break;
                case 1:

                    if (CinematicAspects == true)
                    {
                        RecordResolutionX = 1280;
                        RecordResolutionY = 720;
                    }
                    else
                    {
                        RecordResolutionX = 1024;
                        RecordResolutionY = 512;
                    }
                    break;
                case 2:

                    if (CinematicAspects == true)
                    {
                        RecordResolutionX = 1920;
                        RecordResolutionY = 1080;
                    }
                    else
                    {
                        RecordResolutionX = 2048;
                        RecordResolutionY = 1024;
                    }
                    break;
                case 3:
                    if (CinematicAspects == true)
                    {
                        RecordResolutionX = 3840;
                        RecordResolutionY = 2160;
                    }
                    else
                    {
                        RecordResolutionX = 4096;
                        RecordResolutionY = 2048;
                    }
                    break;
                //Helios 1.0.6
                case 4:
                    if (CinematicAspects == true)
                    {
                        RecordResolutionX = 7680;
                        RecordResolutionY = 4320;
                    }
                    else
                    {
                        RecordResolutionX = 8192;
                        RecordResolutionY = 4096;
                    }
                    break;
                //Helios 1.0.6

                //Helios 1.0.7
                case 5:
                    if (CinematicAspects == true)
                    {
                        RecordResolutionX = 2488;
                        RecordResolutionY = 1400;
                    }
                    else
                    {
                        RecordResolutionX = 2048;
                        RecordResolutionY = 1024;
                    }
                    break;
                //Helios 1.0.7
                default:
                    if (CinematicAspects == true)
                    {
                        RecordResolutionX = 704;
                        RecordResolutionY = 480;
                    }
                    else
                    {
                        RecordResolutionX = 512;
                        RecordResolutionY = 256;
                    }
                    break;
            }

            name = WorkingDir + "/" + name;
            UnityEngine.Debug.Log(name);
            // Open an output stream.
            _pipe = new FFmpegPipe(name, RecordResolutionX, RecordResolutionY, Time.captureFramerate, "argb");

        }

        void ClosePipe()
        {

            _pipe.Close();

        }
        #endregion
    }
}
#endif