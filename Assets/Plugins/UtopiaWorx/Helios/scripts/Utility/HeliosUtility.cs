#define HELIOS3D
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UtopiaWorx;
using UtopiaWorx.Helios;
using UtopiaWorx.Helios.Effects;

namespace UtopiaWorx.Helios
{
	public class HeliosUtility : MonoBehaviour 
	{

		public static string HeliosVersion  = "1.2.6";
		public static string VersionWarnings = "";
		public static void StartRemote()
		{
			if(UnityEditor.EditorApplication.isPlaying == true)
			{
				try
				{
					Helios TheHelios = FindObjectOfType<Helios>();
					TheHelios.RemoteActive = true;
				}
				catch
				{
					
				}

				try
				{
					Helios2D TheHelios = FindObjectOfType<Helios2D>();
					TheHelios.RemoteActive = true;
				}
				catch
				{

				}
			}
		}
		public static void StopRemote()
		{
			if(UnityEditor.EditorApplication.isPlaying == true)
			{
				try
				{
					Helios TheHelios = FindObjectOfType<Helios>();
					TheHelios.RemoteActive = false;
				}
				catch
				{

				}

				try
				{
					Helios2D TheHelios = FindObjectOfType<Helios2D>();
					TheHelios.RemoteActive = false;
				}
				catch
				{

				}
			}
		}
		public static void SetFade(float Amount)
		{
			if(UnityEditor.EditorApplication.isPlaying == true)
			{
				try
				{
					Helios TheHelios = FindObjectOfType<Helios>();
					FadeBlack[] TheFaders = TheHelios.gameObject.GetComponentsInChildren<FadeBlack>();
					for(int i =0; i< TheFaders.Length; i++)
					{
						TheFaders[i].Amount = Amount;  
					}
				}
				catch
				{

				}

				try
				{
					Helios2D TheHelios = FindObjectOfType<Helios2D>();
					FadeBlack TheFader = TheHelios.gameObject.GetComponentInChildren<FadeBlack>();
					TheFader.Amount = Amount;  

				}
				catch
				{

				}
			}
		}
		public static void SetFadeColot(Color FadeColor)
		{
			if(UnityEditor.EditorApplication.isPlaying == true)
			{
				try
				{
					Helios TheHelios = FindObjectOfType<Helios>();
					FadeBlack[] TheFaders = TheHelios.gameObject.GetComponentsInChildren<FadeBlack>();
					for(int i =0; i< TheFaders.Length; i++)
					{
						TheFaders[i].FadeColor = FadeColor;
					}
				}
				catch
				{

				}

				try
				{
					Helios2D TheHelios = FindObjectOfType<Helios2D>();
					FadeBlack TheFader = TheHelios.gameObject.GetComponentInChildren<FadeBlack>();
					TheFader.FadeColor = FadeColor;  

				}
				catch
				{

				}
			}			
		}
	}
}
#endif