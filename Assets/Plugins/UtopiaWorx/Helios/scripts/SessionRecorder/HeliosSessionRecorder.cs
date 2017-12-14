#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace UtopiaWorx.Helios
{
	[HelpURL("http://www.utopiaworx.com/Helios/Helios_Session_Recorder.aspx")]
	[System.Serializable]
	public class HeliosSessionRecorder : MonoBehaviour 
	{
		public Animator TheAnimator;
		public bool EnablePlayback = true;
		public int CursorState = 2;
		public bool SaveAudio = true;
		public System.IO.StreamWriter sw;
		string path = "";
		public static int RecorderState;

		[SerializeField]
		public List<UtopiaWorx.Helios.SessionData> SessionDataCollection;
		[SerializeField]
		public int Frame =0;
		[SerializeField]
		public bool SaveSession = false;
		[SerializeField]
		public string SessionName = "MySession";

		[SerializeField]
		public bool PegasusMode = false;



		private bool SessionExists = false;

		public bool CaptureMouseHoriz = true;
		public bool CaptureMouseVert = true;
		public bool CaptureHoriz = true;
		public bool CaptureVert = true;

		public int TotalFrames;

		private UtopiaWorx.Helios.Helios MyHelios;
		private UtopiaWorx.Helios.Helios2D MyHelios2D;
		private UtopiaWorx.Helios.HeliosController MyHeliosController;
		private UtopiaWorx.Helios.HeliosLook MyHeliosLook;

		private SessionData MySD ;

		void OnDisable()
		{
			if(SaveSession == true)
			{
				if(PegasusMode == false)
				{
				System.Text.StringBuilder SBJSON = new System.Text.StringBuilder();

				foreach(SessionData MYSD in SessionDataCollection)
				{
					SBJSON.Append(JsonUtility.ToJson(MYSD));
					SBJSON.Append("\r\n");

				}
				if(System.IO.Directory.Exists(Application.dataPath + "/UtopiaWorx/") == false)
				{
					System.IO.Directory.CreateDirectory(Application.dataPath + "/UtopiaWorx/");
				}

				if(System.IO.Directory.Exists(Application.dataPath + "/UtopiaWorx/Helios/") == false)
				{
					System.IO.Directory.CreateDirectory(Application.dataPath + "/UtopiaWorx/Helios/");
				}

				if(System.IO.Directory.Exists(Application.dataPath + "/UtopiaWorx/Helios/Sessions/") == false)
				{
					System.IO.Directory.CreateDirectory(Application.dataPath + "/UtopiaWorx/Helios/Sessions/");
				}

				System.IO.File.WriteAllText(Application.dataPath + "/UtopiaWorx/Helios/Sessions/" + SessionName + ".txt",SBJSON.ToString());
				}

				if(SaveAudio == true)
				{
					string OS ="";
					#if UNITY_EDITOR_WIN
					OS = "win";
					#endif

					#if UNITY_EDITOR_OSX
					OS = "osx";
					#endif
					string PathDir = Application.dataPath + "/" + "UtopiaWorx/Helios/Bin/ffmpeg/" + OS + "/";

					//ffmpeg -f s16le -ar 44.1k -ac 2 -i file.pcm file.wav
					string path2 = path.Replace("raw","wav");
					var thread = new Thread(delegate () {Command(PathDir,path,path2);});
					thread.Start();

				}
			}
		}

		// Use this for initialization
		void Start () 
		{
			switch(CursorState)
			{
			case 0:
				Cursor.lockState = CursorLockMode.Confined;
				break;

			case 1:
				Cursor.lockState = CursorLockMode.Locked;
				break;

			case 2:
				Cursor.lockState = CursorLockMode.None;
				break;
			}

			RecorderState = 1;

			MyHelios = gameObject.GetComponent<UtopiaWorx.Helios.Helios>();
			MyHelios2D = gameObject.GetComponent<UtopiaWorx.Helios.Helios2D>();

			try
			{
				TheAnimator = gameObject.GetComponent<Animator>();
			}
			catch
			{
				TheAnimator = null;
			}

			try
			{
				MyHeliosController = gameObject.GetComponent<UtopiaWorx.Helios.HeliosController>();
			}
			catch
			{
				MyHeliosController = null;
			}

			try
			{
				MyHeliosLook = gameObject.GetComponent<UtopiaWorx.Helios.HeliosLook>();
			}
			catch
			{
				MyHeliosLook = null;
			}

			if(SessionName =="")
			{
				SessionName = "MySession";
			}
			if(SaveAudio == true)
			{
				if(System.IO.Directory.Exists(Application.dataPath + "/UtopiaWorx/") == false)
				{
					System.IO.Directory.CreateDirectory(Application.dataPath + "/UtopiaWorx/");
				}

				if(System.IO.Directory.Exists(Application.dataPath + "/UtopiaWorx/Helios/") == false)
				{
					System.IO.Directory.CreateDirectory(Application.dataPath + "/UtopiaWorx/Helios/");
				}

				if(System.IO.Directory.Exists(Application.dataPath + "/UtopiaWorx/Helios/Sessions/") == false)
				{
					System.IO.Directory.CreateDirectory(Application.dataPath + "/UtopiaWorx/Helios/Sessions/");
				}

				path = Application.dataPath + "/UtopiaWorx/Helios/Sessions/" + SessionName + ".raw";
			}
			if(SessionDataCollection == null)
			{
				SessionDataCollection = new List<UtopiaWorx.Helios.SessionData>();
			}
			Frame =0;

			if(SaveSession == false)
			{
				
				if(System.IO.File.Exists(Application.dataPath + "/UtopiaWorx/Helios/Sessions/" + SessionName + ".txt"))
				{
					SessionExists = true;
				}
				if(SessionExists)
				{
					string[] JsonData = System.IO.File.ReadAllLines(Application.dataPath + "/UtopiaWorx/Helios/Sessions/" + SessionName + ".txt");
					foreach(string MyJson in JsonData)
					{
						UtopiaWorx.Helios.SessionData Data =  (UtopiaWorx.Helios.SessionData)JsonUtility.FromJson(MyJson,typeof(UtopiaWorx.Helios.SessionData));
						SessionDataCollection.Add(Data);
					}
					TotalFrames = JsonData.Length;
				}
			}
		}
		
		// Update is called once per frame
		void FixedUpdate () 
		{
			if(SaveSession== true)
			{
				if(PegasusMode == false)
				{
			 MySD = new UtopiaWorx.Helios.SessionData();
				if(CaptureVert == true)
				{
					if(Input.GetKey(KeyCode.W ) == true || Input.GetKey(KeyCode.UpArrow) == true)
					{
						MySD.W = true;
					}
				}
				else
				{
					MySD.W = false;
				}

				if(CaptureHoriz == true)
				{
					if(Input.GetKey(KeyCode.A) == true || Input.GetKey(KeyCode.LeftArrow) == true)
					{
						MySD.A = true;
					}
				}
				else
				{
					MySD.A = false;	
				}

				if(CaptureVert == true)
				{
					if(Input.GetKey(KeyCode.S) == true || Input.GetKey(KeyCode.DownArrow) == true)
					{
						MySD.S = true;
					}

				}
				else
				{
					MySD.S = false;
				}
				if(CaptureHoriz == true)
				{
					if(Input.GetKey(KeyCode.D) == true || Input.GetKey(KeyCode.RightArrow) == true)
					{
						MySD.D = true;
					}
				}
				else
				{
					MySD.D = false;
				}

			if(Input.GetKey(KeyCode.LeftShift) == true)
			{
				MySD.Shift = true;
			}
			if(Input.GetKey(KeyCode.Space) == true)
			{
				MySD.Space = true;
			}

				if(CaptureMouseHoriz == true)
				{
					MySD.mouseX =Input.GetAxis("Mouse X");
				}
				else
				{
					MySD.mouseX =0.0f;
				}
				if(CaptureMouseVert == true)
				{
					MySD.mouseY = Input.GetAxis("Mouse Y");					
				}
				else
				{
					MySD.mouseY = 0.0f;									
				}

				MySD.Position = transform.position;
                    MySD.Scale = transform.localScale;

//					float LocalX = transform.eulerAngles.x;
//					float LocalY = transform.eulerAngles.y;
//					float LocalZ = transform.eulerAngles.z;
//					if(LocalX > 360.0f)
//					{
//						float SubTractThis = LocalX - 360.0f;
//						LocalX = LocalX - SubTractThis;
//					}
//					if(LocalX < -360.0f)
//					{
//						float AddThis = LocalX + 360.0f;
//						LocalX = LocalX + AddThis;
//					}
//
//					if(LocalY > 360.0f)
//					{
//						float SubTractThis = LocalY - 360.0f;
//						LocalY = LocalY - SubTractThis;
//					}
//					if(LocalY < -360.0f)
//					{
//						float AddThis = LocalY + 360.0f;
//						LocalY = LocalY + AddThis;
//					}
//
//					if(LocalZ > 360.0f)
//					{
//						float SubTractThis = LocalZ - 360.0f;
//						LocalZ = LocalZ - SubTractThis;
//					}
//					if(LocalZ < -360.0f)
//					{
//						float AddThis = LocalZ + 360.0f;
//						LocalZ = LocalZ + AddThis;
//					}
//
//
//					MySD.Rotation = new Vector3(LocalX,LocalY,LocalZ);
					MySD.Rotation = transform.rotation;
			MySD.FrameID = Frame;

					#region Animation
					if(TheAnimator != null)
					{	
						if(TheAnimator.parameters.Length >0)
						{
							MySD.AnimationDetails = new List<SessionDataAnimationDetails>();
							foreach(AnimatorControllerParameter MyParam in TheAnimator.parameters)
							{
								SessionDataAnimationDetails NewData = new SessionDataAnimationDetails();
								NewData.NameValue = MyParam.name;

								switch(MyParam.type)
								{
								case AnimatorControllerParameterType.Bool: 
									NewData.BoolValue = TheAnimator.GetBool(MyParam.name);
									NewData.Type = AnimatorControllerParameterType.Bool;
									break;
								case AnimatorControllerParameterType.Float: 
									NewData.FloatValue = TheAnimator.GetFloat(MyParam.name);
									NewData.Type = AnimatorControllerParameterType.Float;
									break;
								case AnimatorControllerParameterType.Int: 
									NewData.IntValue = TheAnimator.GetInteger(MyParam.name);
									NewData.Type = AnimatorControllerParameterType.Int;
									break;
								}

								MySD.AnimationDetails.Add(NewData);
							}
						}
					}
					#endregion
			SessionDataCollection.Add(MySD);
				}
			}
			else
			{
				if(SessionExists && EnablePlayback == true)
				{
					if(MyHelios != null)
					{
						try
						{

							float Percent = ((float)Frame / (float)TotalFrames * 100.0f);
							MyHelios.ProgressData = "Frame " + Frame.ToString() + " of " + TotalFrames.ToString() + " (" + decimal.Round((decimal)Percent,2).ToString() + " %) ";
						}
						catch
						{
							
						}
					}

					if(MyHelios2D != null)
					{
						try
						{

							float Percent = ((float)Frame / (float)TotalFrames * 100.0f);
							MyHelios2D.ProgressData = "Frame " + Frame.ToString() + " of " + TotalFrames.ToString() + " (" + decimal.Round((decimal)Percent,2).ToString() + " %) ";
						}
						catch
						{

						}
					}

					try
					{
						MyHeliosController.SetFrameInput(SessionDataCollection[Frame ]);
					}
					catch
					{
						#if UNITY_EDITOR
						UnityEditor.EditorApplication.isPlaying = false;
						bool ShowDialog = false;
						try
						{
							if(MyHelios.IsArmed == true)
							{
								ShowDialog = true;
							}
						}
						catch
						{

						}

						try
						{
							if(MyHelios2D.IsArmed == true)
							{
								ShowDialog = true;
							}
						}
						catch
						{


						}
						if(ShowDialog == true)
						{

							try
							{
								MyHelios.IsArmed = false;
							}
							catch
							{}
							try
							{
								MyHelios2D.IsArmed = false;
							}
							catch
							{}
							UnityEditor.EditorUtility.DisplayDialog("Helios is Complete","The image capture portion of your video is now complete, please click 'Create Video' button to encode your images to video format.","Got It!");
						}
						#endif
					}
					try
					{
						MyHeliosLook.SetFrameInput(SessionDataCollection[Frame]);
					}
					catch
					{}

				}
			}
			Frame++;
		}

		static void Command (string RunPath, string RawDataPath, string Output)
		{

			#if UNITY_EDITOR_OSX 
			string args = "-y -f f32le -ar 44.1k -ac 2 -i \"" + RawDataPath + "\" \"" + Output + "\"";

			var processInfo = new ProcessStartInfo( RunPath + "ffmpeg",args);
			processInfo.CreateNoWindow = false;
			var process = Process.Start(processInfo);
			process.WaitForExit();
			process.Close();
			RecorderState = 2;

			#endif

			#if UNITY_EDITOR_WIN
			string args = "-y -f f32le -ar 44.1k -ac 2 -i \"" + RawDataPath.Replace(@"/",@"\") + "\" \"" + Output.Replace(@"/",@"\") +"\"";

			var processInfo = new ProcessStartInfo( RunPath + "ffmpeg",args);
			processInfo.CreateNoWindow = false;
			var process = Process.Start(processInfo);
			process.WaitForExit();
			process.Close();
			RecorderState = 2;
			#endif

		}
	
		void OnAudioFilterRead(float[] data, int channels)
		{
			if(SaveAudio == true )
			{
				if (path!="") 
				{ 
					BinaryWriter sw = new BinaryWriter(File.Open(path, FileMode.Append));
					foreach(float MyFloat in data)
					{
						sw.Write(MyFloat);
					}
					sw.Close();
				}
			}
		}

	}
}
#endif