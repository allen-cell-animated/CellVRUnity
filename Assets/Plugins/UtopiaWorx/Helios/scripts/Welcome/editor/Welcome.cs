#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;


namespace UtopiaWorx.Helios
{
	[InitializeOnLoad]
	public class Welcome : EditorWindow {

		[MenuItem ("Window/Helios/Install FFMPEG")]
		static void InstallFFMPEG()
		{
			EditorUtility.DisplayDialog("Install FFMPEG","This will take you to our DropBox where you can download a copy of FFMPEG. Once you have downloaded it, import the .unitypackage into your project.","OK");
			Application.OpenURL("https://www.dropbox.com/s/lavnhc6zgplqhb7/Helios_Auxilary.unitypackage?dl=0");
			
		}
		#if UNITY_EDITOR_OSX
		[MenuItem ("Window/Helios/Remove Windows Files")]
		static void RemoveWindows()
		{
			int RetVal = EditorUtility.DisplayDialogComplex("Delete Unneeded Windows Files","Helios ships with support for both Windows and OSX. Each of these operating systems has slightly different needs in terms of resources. If you are on OSX you can go ahead and delete the Windows files and free up some space. Would you like to delete these files? (You can reinstall Helios to get them back.)","Yes, Delete Windows Files!","No, I will hang onto them.","asdfasdf");
			if(RetVal == 0)
			{
				System.IO.Directory.Delete(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/",true);				
				System.IO.Directory.Delete(Application.dataPath + "/" + "UtopiaWorx/Helios/Bin/ffmpeg/win/",true);	
			}
		}
		#endif

		#if UNITY_EDITOR_WIN
		[MenuItem ("Window/Helios/Remove OSX Files")]
		static void RemoveOSX()
		{
			int RetVal = EditorUtility.DisplayDialogComplex("Delete Unneeded Windows Files","Helios ships with support for both Windows and OSX. Each of these operating systems has slightly different needs in terms of resources. If you are on Windows you can go ahead and delete the OSX files and free up some space. Would you like to delete these files? (You can reinstall Helios to get them back.)","Yes, Delete OSX Files!","No, I will hang onto them.","");
			if(RetVal == 0)
			{
				System.IO.Directory.Delete(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/",true);					
				System.IO.Directory.Delete(Application.dataPath + "/" + "UtopiaWorx/Helios/Bin/ffmpeg/osx/",true);	
			}
		}
		#endif
		[MenuItem ("Window/Helios/Rebuild Resources")]
		public static void RebuildResources()
		{
			#region Create Cubemaps directory
			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/Cubemaps") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/Cubemaps/");
			}
			#endregion

			#region Setup Cubemaps
			if(System.IO.File.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/Cubemaps/480.cubemap") == false)
			{
				Cubemap CM480 = new Cubemap(256,TextureFormat.ARGB32,false);
				AssetDatabase.CreateAsset(CM480,"Assets/" + "UtopiaWorx/Helios/Cubemaps/480.cubemap");
			}
			if(System.IO.File.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/Cubemaps/480_R.cubemap") == false)
			{
				Cubemap CM480_R = new Cubemap(256,TextureFormat.ARGB32,false);
				AssetDatabase.CreateAsset(CM480_R,"Assets/" + "UtopiaWorx/Helios/Cubemaps/480_R.cubemap");
			}

			if(System.IO.File.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/Cubemaps/720.cubemap") == false)
			{
				Cubemap CM720 = new Cubemap(512,TextureFormat.ARGB32,false);
				AssetDatabase.CreateAsset(CM720,"Assets/" + "UtopiaWorx/Helios/Cubemaps/720.cubemap");
			}

			if(System.IO.File.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/Cubemaps/720_R.cubemap") == false)
			{
				Cubemap CM720_R = new Cubemap(512,TextureFormat.ARGB32,false);
				AssetDatabase.CreateAsset(CM720_R,"Assets/" + "UtopiaWorx/Helios/Cubemaps/720_R.cubemap");
			}

			if(System.IO.File.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/Cubemaps/1080.cubemap") == false)
			{
				Cubemap CM1080 = new Cubemap(1024,TextureFormat.ARGB32,false);
				AssetDatabase.CreateAsset(CM1080,"Assets/" + "UtopiaWorx/Helios/Cubemaps/1080.cubemap");
			}

			if(System.IO.File.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/Cubemaps/1080_R.cubemap") == false)
			{
				Cubemap CM1080_R = new Cubemap(1024,TextureFormat.ARGB32,false);
				AssetDatabase.CreateAsset(CM1080_R,"Assets/" + "UtopiaWorx/Helios/Cubemaps/1080_R.cubemap");
			}

			if(System.IO.File.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/Cubemaps/4K.cubemap") == false)
			{
				Cubemap CM4K = new Cubemap(2048,TextureFormat.ARGB32,false);
				AssetDatabase.CreateAsset(CM4K,"Assets/" + "UtopiaWorx/Helios/Cubemaps/4K.cubemap");
			}

			if(System.IO.File.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/Cubemaps/4K_R.cubemap") == false)
			{
				Cubemap CM4K_R = new Cubemap(2048,TextureFormat.ARGB32,false);
				AssetDatabase.CreateAsset(CM4K_R,"Assets/" + "UtopiaWorx/Helios/Cubemaps/4K_R.cubemap");
			}
			#endregion

			#region Setup Render Texture Directories
			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic/");
			}
			#endregion

			#region Setup Render Textures

			#region OSX

			#region Cinematic

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/iPad") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/iPad/");
				RenderTexture RTOSXCIN1400 = new RenderTexture(1538,2048,24,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Default);
				RTOSXCIN1400.filterMode = FilterMode.Bilinear;
				RTOSXCIN1400.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN1400.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN1400,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/iPad/Front.renderTexture");
			}
			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/iPhone5") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/iPhone5/");
				RenderTexture RTOSXCIN1400 = new RenderTexture(640,1136,24,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Default);
				RTOSXCIN1400.filterMode = FilterMode.Bilinear;
				RTOSXCIN1400.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN1400.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN1400,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/iPhone5/Front.renderTexture");
			}
			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/iPhone6") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/iPhone6/");
				RenderTexture RTOSXCIN1400 = new RenderTexture(750,1334,24,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Default);
				RTOSXCIN1400.filterMode = FilterMode.Bilinear;
				RTOSXCIN1400.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN1400.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN1400,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/iPhone6/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/1400") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/1400/");
				RenderTexture RTOSXCIN1400 = new RenderTexture(2488,1400,24,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Default);
				RTOSXCIN1400.filterMode = FilterMode.Bilinear;
				RTOSXCIN1400.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN1400.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN1400,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/1400/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/8K") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/8K/");
				RenderTexture RTOSXCIN8K = new RenderTexture(7860,4320,24,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/8K/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/Extreme") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/Extreme/");

				RenderTexture RTOSXCIN8K = new RenderTexture(3840,2160,24,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/Extreme/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/High") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/High/");

				RenderTexture RTOSXCIN8K = new RenderTexture(1920,1080,24,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/High/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/Low") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/Low/");

				RenderTexture RTOSXCIN8K = new RenderTexture(704,480,24,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/Low/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/Medium") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/Medium/");

				RenderTexture RTOSXCIN8K = new RenderTexture(1280,720,24,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cinematic/Medium/Front.renderTexture");
			}

			#endregion

			#region Cubic
			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic/1400") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic/1400/");
				RenderTexture RTOSXCIN1400 = new RenderTexture(2488,1400,24,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Default);
				RTOSXCIN1400.filterMode = FilterMode.Bilinear;
				RTOSXCIN1400.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN1400.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN1400,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic/1400/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic/8K") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic/8K/");

				RenderTexture RTOSXCIN8K = new RenderTexture(8192,4096,24,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic/8K/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic/Extreme") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic/Extreme/");
				RenderTexture RTOSXCIN8K = new RenderTexture(4096,2048,24,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic/Extreme/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic/High") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic/High/");

				RenderTexture RTOSXCIN8K = new RenderTexture(2048,1024,24,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic/High/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic/Low") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic/Low/");
				RenderTexture RTOSXCIN8K = new RenderTexture(512,256,24,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic/Low/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic/Medium") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic/Medium/");

				RenderTexture RTOSXCIN8K = new RenderTexture(1024,512,24,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/OSX/Cubic/Medium/Front.renderTexture");
			}
			#endregion
		#endregion

			#endregion

			#region Setup Render Texture Directories
			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic/");
			}
			#endregion

			#region setup Render Textures

			#region Windows
			#region Cinematic

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/iPad") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/iPad/");
				RenderTexture RTOSXCIN1400 = new RenderTexture(1538,2048,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default);
				RTOSXCIN1400.filterMode = FilterMode.Bilinear;
				RTOSXCIN1400.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN1400.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN1400,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/iPad/Front.renderTexture");
			}
			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/iPhone5") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/iPhone5/");
				RenderTexture RTOSXCIN1400 = new RenderTexture(640,1136,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default);
				RTOSXCIN1400.filterMode = FilterMode.Bilinear;
				RTOSXCIN1400.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN1400.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN1400,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/iPhone5/Front.renderTexture");
			}
			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/iPhone6") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/iPhone6/");
				RenderTexture RTOSXCIN1400 = new RenderTexture(750,1334,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default);
				RTOSXCIN1400.filterMode = FilterMode.Bilinear;
				RTOSXCIN1400.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN1400.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN1400,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/iPhone6/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/1400") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/1400/");
				RenderTexture RTOSXCIN1400 = new RenderTexture(2488,1400,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default);
				RTOSXCIN1400.filterMode = FilterMode.Bilinear;
				RTOSXCIN1400.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN1400.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN1400,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/1400/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/8K") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/8K/");
				RenderTexture RTOSXCIN8K = new RenderTexture(7860,4320,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/8K/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/Extreme") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/Extreme/");

				RenderTexture RTOSXCIN8K = new RenderTexture(3840,2160,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/Extreme/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/High") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/High/");

				RenderTexture RTOSXCIN8K = new RenderTexture(1920,1080,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/High/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/Low") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/Low/");

				RenderTexture RTOSXCIN8K = new RenderTexture(704,480,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/Low/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/Medium") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/Medium/");

				RenderTexture RTOSXCIN8K = new RenderTexture(1280,720,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cinematic/Medium/Front.renderTexture");
			}
			#endregion

			#region Cubic

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic/1400") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic/1400/");

				RenderTexture RTOSXCIN1400 = new RenderTexture(2048,1024,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default);
				RTOSXCIN1400.filterMode = FilterMode.Bilinear;
				RTOSXCIN1400.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN1400.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN1400,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic/1400/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic/8K") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic/8K/");

				RenderTexture RTOSXCIN8K = new RenderTexture(8192,4096,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic/8K/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic/Extreme") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic/Extreme/");
				RenderTexture RTOSXCIN8K = new RenderTexture(4096,2048,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic/Extreme/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic/High") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic/High/");

				RenderTexture RTOSXCIN8K = new RenderTexture(2048,1024,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic/High/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic/Low") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic/Low/");
				RenderTexture RTOSXCIN8K = new RenderTexture(512,256,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic/Low/Front.renderTexture");
			}

			if(System.IO.Directory.Exists(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic/Medium") == false)
			{
				System.IO.Directory.CreateDirectory(Application.dataPath + "/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic/Medium/");

				RenderTexture RTOSXCIN8K = new RenderTexture(1024,512,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default);
				RTOSXCIN8K.filterMode = FilterMode.Bilinear;
				RTOSXCIN8K.wrapMode = TextureWrapMode.Clamp;
                RTOSXCIN8K.antiAliasing = 8;
                AssetDatabase.CreateAsset(RTOSXCIN8K,"Assets/" + "UtopiaWorx/Helios/RenderTextures/2D/Windows/Cubic/Medium/Front.renderTexture");
			}

			#endregion
			#endregion
			#endregion
		}

        
        void OnEnable()
        {
            //Welcome MyWelcom = new Welcome();
        }

        static Welcome()
		{


			

			


		}
		// Add menu named "My Window" to the Window menu
		[MenuItem ("Window/Helios/About Helios")]
		static void Init () {

            bool Shown = EditorPrefs.GetBool("HeliosWelcomeShown" + UtopiaWorx.Helios.HeliosUtility.HeliosVersion);
            if (Shown == false)
            {
                EditorPrefs.SetBool("HeliosWelcomeShown" + UtopiaWorx.Helios.HeliosUtility.HeliosVersion, true);
                bool RetVal = EditorUtility.DisplayDialog("Install FFMPEG", "Due to licensing issues, Helios is no longer able to be distributed from the Unity Asset Store with the FFMPEG binaries included. However you can simply install them right now by clicking the \"Install FFMPEG\" button below.", "Install FFMPEG", "I dont want FFMPEG");
                if (RetVal == true)
                {
                    Welcome.InstallFFMPEG();
                }
                else
                {
                    EditorUtility.DisplayDialog("FFMPEG", "I Understand that by not installing FFMPEG, Helios will not be able to compose videos or GIFs, but only record still images. You can always install them later by clicking on Window/Helios/Install FFMPEG.", "Got it");
                }

                if (UtopiaWorx.Helios.HeliosUtility.VersionWarnings.Length > 0)
                {
                    EditorUtility.DisplayDialog("Helios Update Specific Information", "We know that everyone hates these pop up messages, but this version of Helios has some specific changes you need to know about:\r\n\r\n" + UtopiaWorx.Helios.HeliosUtility.VersionWarnings, "Got It!");
                }
            //    Init();
            }

            UtopiaWorx.Helios.Welcome.RebuildResources();

            // Get existing open window or if none, make a new one:
            Welcome window = (Welcome)EditorWindow.GetWindow (typeof (Welcome),true,"About Helios");
			window.minSize = new Vector2(400,620);
			window.maxSize = new Vector2(400,620);
			window.ShowUtility();
			//window.Show();
		}


		Vector2 SVPos = new Vector2(0,0);
		void OnGUI () 
		{
			Texture2D textureLogo = (Texture2D)EditorGUIUtility.Load("Assets/UtopiaWorx/Helios/Resources/Images/Helios.png");
			Texture2D SupportLogo = (Texture2D)EditorGUIUtility.Load("Assets/UtopiaWorx/Helios/Resources/Images/online_support-128.png");
			Texture2D YouTubeLogo = (Texture2D)EditorGUIUtility.Load("Assets/UtopiaWorx/Helios/Resources/Images/youtube-128.png");
			Texture2D WebsiteLogo = (Texture2D)EditorGUIUtility.Load("Assets/UtopiaWorx/Helios/Resources/Images/internet-128.png");
			Texture2D FilmLogo = (Texture2D)EditorGUIUtility.Load("Assets/UtopiaWorx/Helios/Resources/Images/film_reel-128.png");

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(textureLogo);
			GUILayout.EndHorizontal();
			GUILayout.Label("Thank you for purchasing Helios!");
			GUILayout.Label("Version: " + UtopiaWorx.Helios.HeliosUtility.HeliosVersion) ;
			GUILayout.Label("Support:");
			GUILayout.Label("  Email:john@smarterphonelabs.com");
			GUILayout.Label("  Skype:JohnRossitter");
            GUILayout.Label("  Skype real time support : https://join.skype.com/OvTh8kiB7YPz");
            GUILayout.Label("  Twitter:@MaterializerPBR");

			GUILayout.Label("");
			GUILayout.Label("Please consider rating and reviewing Helios!");
			GUILayout.Label("");
			GUILayout.Label("Here are a few resources for you:");

			// Forum
			GUILayout.Label("");
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(WebsiteLogo,GUILayout.Width(50),GUILayout.Height(50)))
			{
				Application.OpenURL("http://www.helios3dvideo.com");
			}
			GUILayout.Label("Helios Website:\r\nOur official website");
			GUILayout.EndHorizontal();

			GUILayout.Label("");
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(SupportLogo,GUILayout.Width(50),GUILayout.Height(50)))
			{
				Application.OpenURL("http://forum.unity3d.com/threads/helios-3d-360-video.408218/");
			}
			GUILayout.Label("Unity Support Forum:\r\nLot's of great discussion about the general \r\nusage of Helios.");
			GUILayout.EndHorizontal();

			// Forum
			GUILayout.Label("");
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(YouTubeLogo,GUILayout.Width(50),GUILayout.Height(50)))
			{
				Application.OpenURL("https://www.youtube.com/watch?v=oAi5vdGggtY&list=PLqByiwoiu0WBzBl0C0YPthjexH6hUXmJb");
			}
			GUILayout.Label("YouTube Playlist:\r\nGreat collection of videos which \r\ndemonstrate how to use Helios.");
			GUILayout.EndHorizontal();

			// Forum
			GUILayout.Label("");
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(FilmLogo,GUILayout.Width(50),GUILayout.Height(50)))
			{
				Application.OpenURL("http://www.kolor.com/kolor-eyes/#download");
			}
			GUILayout.Label("Local 360 Viewing Software:\r\nThis is an app which you can use \r\nto view Helios files with.");
			GUILayout.EndHorizontal();



			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("What's new in this version:");
			EditorGUILayout.EndHorizontal();
			SVPos = GUILayout.BeginScrollView(SVPos);
			GUILayout.TextArea("-Added support for controlling VideoPlayer playback speed in Unity 2017");
			GUILayout.EndScrollView();

		}
	}
}

#endif