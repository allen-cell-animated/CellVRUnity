using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Cell;

public class CellViveHead : MonoBehaviour
{
    public bool canSwitch = true;
    public SteamVR_LoadLevel levelLoader;
    public List<CellViveController> controllers = new List<CellViveController>();
	public GameObject fadeText;
	public GameObject uiCamera;
	public Transform eyes;

	bool fading = false;
	float fadeDuration = 2f;
	float startFadeTime = -100f;
	Cell currentCell;

    bool canSwitchScene
    {
        get
        {
            return canSwitch && controllers.Find( c => c.state == CellViveControllerState.Scaling ) == null 
                && controllers.Find( c => c.state == CellViveControllerState.HoldingCell ) != null;
        }
    }

    void OnTriggerEnter (Collider other)
    {
        if (canSwitchScene)
        {
			currentCell = other.GetComponentInParent<Cell>();
			if (currentCell != null)
            {
                StartFade();
            }
        }
	}

	void OnTriggerExit (Collider other)
    {
        if (fading && other.GetComponentInParent<Cell>() == currentCell)
        {
            StopFade();
			currentCell = null;
		}
	}

	void StartFade ()
	{
		fadeText.transform.position = Camera.main.transform.position + 300f * Camera.main.transform.forward;
		fadeText.transform.rotation = Quaternion.LookRotation( fadeText.transform.position - Camera.main.transform.position );
		fadeText.SetActive( true );
		uiCamera.SetActive( true );
		uiCamera.transform.position = eyes.position;
		uiCamera.transform.rotation = eyes.rotation;

		SteamVR_Fade.Start(Color.clear, 0f);
		SteamVR_Fade.Start(Color.black, fadeDuration);

		startFadeTime = Time.time;
		fading = true;
	}

	void StopFade ()
	{
		fadeText.SetActive( false );
		SteamVR_Fade.Start(Color.clear, 0f);
		uiCamera.SetActive( false );
		fading = false;
	}
}
