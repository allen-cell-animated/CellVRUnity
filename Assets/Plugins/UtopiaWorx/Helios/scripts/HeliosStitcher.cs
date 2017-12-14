
#if UNITY_EDITOR
using UnityEngine;

using UnityEditor;
using System;
using UtopiaWorx;

namespace UtopiaWorx
{
	[HelpURL("http://www.utopiaworx.com/Helios/Helios_Stitcher.aspx")]
	[RequireComponent (typeof (UnityEngine.Camera))]
	class HeliosStitcher : MonoBehaviour
	{
		
		private Camera cam;
		private Cubemap CM;
		private UtopiaWorx.Helios.Helios MyHelios;
//Helios 1.0.6
		public bool CameraLeft;
//Helios 1.0.6


		void Start()
		{
			
			MyHelios = transform.parent.gameObject.GetComponent<UtopiaWorx.Helios.Helios>();
			cam = gameObject.GetComponent<Camera>();
			cam.useOcclusionCulling = false;
			cam.farClipPlane = 10000.0f;
			cam.nearClipPlane = 0.01f;
			if(MyHelios.IsArmed == true)
			{
				string CubePath = "";
				switch(MyHelios.Quality)
				{
//Helios 1.0.6
				case 0:
					if(CameraLeft == false)
					{
						CubePath = "Cubemaps/480_R.cubemap";
					}
					else
					{
						CubePath = "Cubemaps/480.cubemap";
					}
					break;
				case 1:
					
					if(CameraLeft == false)
					{
						CubePath = "Cubemaps/720_R.cubemap";
					}
					else
					{
						CubePath = "Cubemaps/720.cubemap";
					}
					break;
				case 2:
					if(CameraLeft == false)
					{
						CubePath = "Cubemaps/1080_R.cubemap";
					}
					else
					{
						CubePath = "Cubemaps/1080.cubemap";
					}
					break;
				case 3:
					if(CameraLeft == false)
					{
						CubePath = "Cubemaps/4K_R.cubemap";
					}
					else
					{
						CubePath = "Cubemaps/4K.cubemap";
					}
					break;

				case 4:
					if(CameraLeft == false)
					{
						CubePath = "Cubemaps/4K_R.cubemap";
					}
					else
					{
						CubePath = "Cubemaps/4K.cubemap";
					}
					break;
//Helios 1.0.6
//Helios 1.0.7
				case 5:
					if(CameraLeft == false)
					{
						CubePath = "Cubemaps/4K_R.cubemap";
					}
					else
					{
						CubePath = "Cubemaps/4K.cubemap";
					}
					break;
//Helios 1.0.7
				}
				CM = (Cubemap)AssetDatabase.LoadAssetAtPath("Assets/UtopiaWorx/Helios/" + CubePath,typeof(Cubemap));
				//CM = (Cubemap)Resources.Load(CubePath) as Cubemap;
				BuildCube(63);
			}


		}

		void FixedUpdate()
		{
			
			if(MyHelios.IsArmed == true)
			{
				bool CanRecord = false;
				switch(MyHelios.RecordMode)
				{
				case 0:
					CanRecord = true;
					break;
				case 1:
					if(MyHelios.TheFrame >= MyHelios.StartFrame && MyHelios.TheFrame <= MyHelios.EndFrame)
					{
						CanRecord = true;
					}
					break;
				case 2:
					if(Input.GetKey(MyHelios.HotKey) == true)
					{
						CanRecord = true;
					}
					break;
				case 3:
					if(MyHelios.RemoteActive == true)
					{
						CanRecord = true;
					}
					break;
//Helios 1.0.6
				case 4:
					if(MyHelios.TheFrame <1)
					{
						CanRecord = true;
					}
//Helios 1.0.6
					break;
				}
				if(CameraLeft == false && MyHelios.ProjectionMode !=2)
				{
					CanRecord = false;
				}
				if(CanRecord == true)
				{
					BuildCube(63);
				}
			}

		}
		void BuildCube(int faceMask)
		{

			cam.RenderToCubemap(CM, faceMask);
			if(MyHelios.EdgeStitching > 0)
			{
				CM.SmoothEdges(MyHelios.EdgeStitching);
			}
			CM.Apply();
		}



	}
}
#endif