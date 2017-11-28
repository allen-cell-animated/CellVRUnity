using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.Cell;

public class CellViveHead : MonoBehaviour
{
    public bool canSwitch = true;
    public SteamVR_LoadLevel levelLoader;
    public List<CellViveController> controllers = new List<CellViveController>();
	public MeshRenderer fader;
	public GameObject fadeText;

	bool fading = false;
	Color faderColor;
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

	void Start ()
	{
		faderColor = fader.material.color;
	}

    void OnTriggerEnter (Collider other)
    {
        if (canSwitchScene)
        {
			currentCell = other.GetComponent<Cell>();
			if (currentCell != null)
            {
				StartFade();
            }
        }
	}

	void OnTriggerExit (Collider other)
	{
		if (fading && other.GetComponent<Cell>() == currentCell)
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
		fader.gameObject.SetActive( true );

		SetFader( 0 );

		startFadeTime = Time.time;
		fading = true;
	}

	void Update ()
	{
		if (fading) 
		{
			float t = (Time.time - startFadeTime) / fadeDuration;

			if (t >= 1)
			{
				StopFade();
				levelLoader.Trigger();
			}
			else
			{
				SetFader( t );
			}
		}
	}

	void StopFade ()
	{
		fadeText.SetActive( false );
		fader.gameObject.SetActive( false );
		fading = false;
	}

	void SetFader (float t)
	{
		fader.material.color = new Color( faderColor.r, faderColor.g, faderColor.b, t );
	}
}
