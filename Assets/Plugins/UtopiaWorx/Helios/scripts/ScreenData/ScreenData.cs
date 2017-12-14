#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

public class ScreenData : MonoBehaviour {

	private int TheFrame;
	public UnityEngine.UI.Text TargetText;

	public UtopiaWorx.Helios.Helios H;
	public UtopiaWorx.Helios.Helios2D H2;

	void Start()
	{
		try
		{
			H = FindObjectOfType<UtopiaWorx.Helios.Helios>();
		}
		catch
		{}

		try
		{
			H2 = FindObjectOfType<UtopiaWorx.Helios.Helios2D>();
		}
		catch
		{}
	}
	// Update is called once per frame
	void Update () 
	{
		string IsRecording = "";
		bool CanRecord = false;

		if(H != null)
		{
			if(H.IsArmed == true)
			{
			switch(H.RecordMode)
			{
			case 0:
				CanRecord = true;
				break;
			case 1:
				if(H.TheFrame >= H.StartFrame && H.TheFrame <= H.EndFrame)
				{
					CanRecord = true;
				}
				break;
			case 2:
				if(H.HotKeyEnabled == true)
				{
					CanRecord = true;
				}
				break;
			case 3:
				if(H.RemoteActive == true)
				{
					CanRecord = true;
				}
				break;

			case 4:
				if(H.TheFrame <1)
				{
					CanRecord = true;
				}
				break;

			}
			}

		}

		if(H2 != null)
		{
			if(H2.IsArmed == true)
			{
				switch(H2.RecordMode)
				{
				case 0:
					CanRecord = true;
					break;
				case 1:
					if(H2.TheFrame >= H2.StartFrame && H2.TheFrame <= H2.EndFrame)
					{
						CanRecord = true;
					}
					break;
				case 2:
					if(H2.HotKeyEnabled == true)
					{
						CanRecord = true;
					}
					break;
				case 3:
					if(H2.RemoteActive == true)
					{
						CanRecord = true;
					}
					break;

				case 4:
					if(H2.TheFrame <1)
					{
						CanRecord = true;
					}
					break;

				}
			}

		}

		if(CanRecord == true)
		{
			IsRecording = "RECORDING!!!    ";
		}
		TargetText.text = IsRecording +  "Helios is Running - Frame: " + TheFrame.ToString() + " Game Play Time: " + Time.timeSinceLevelLoad.ToString() + " Recording at " + Time.captureFramerate.ToString() + "FPS";
		TheFrame++;
	}
}
#endif