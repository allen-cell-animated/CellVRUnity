using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AICS.MotorProteins;
using AICS.MotorProteins.Kinesin;
using AICS.MT;
using UnityEngine.SceneManagement;

public class KinesinViveController : ViveController
{
    public Kinesin kinesin;
    public Microtubule microtubule;
    public SteamVR_PlayArea playArea;
    public KinesinViveController otherController;
    public SteamVR_LoadLevel levelLoader;
	public MeshRenderer fader;
	public GameObject fadeText;
	public TimeUI timeUI;
    public float minTimeMultiplier = 1f;
    public float maxTimeMultiplier = 10000f;
    public float scaleMultiplier = 0.1f;
    public float minScale = 15f;
    public float maxScale = 110f;
	public float startFadeScale = 76f;
	public float fadeTextScale = 95f;
    public float scaleSpeed = 0.1f;
    public bool holdingTrigger = false;
//	public GameObject pushButtonLabel;
	public GameObject scaleButtonLabel;
	public LineRenderer scaleLine;
//	public GameObject pushIndicator;
	public GameObject labelLine;
	public GameObject uiCamera;
	public Transform eyes;

    bool scaling = false;
    float startControllerDistance;
    float startScale;
	Color faderColor;
//	Collider theCollider;
	Vector3[] linePoints = new Vector3[2];

	bool idle 
	{
		get
		{
			return !scaling && !holdingTrigger;
		}
	}

	void Start ()
	{
		faderColor = fader.material.color;
		if (timeUI != null)
		{
			timeUI.minTimeMultiplier = minTimeMultiplier;
			timeUI.maxTimeMultiplier = maxTimeMultiplier;
			timeUI.Set( MolecularEnvironment.Instance.timeMultiplier );
		}
//		theCollider = GetComponent<Collider>();
//		theCollider.enabled = false;
//		pushIndicator.SetActive( false );
	}

    public override void OnTriggerPull()
    {
        holdingTrigger = true;
//		pushIndicator.SetActive( true );
//		theCollider.enabled = true;
        if (otherController.holdingTrigger)
        {
            StartScaling();
        }
    }

    public override void OnTriggerHold()
    {
        if (scaling)
        {
            UpdateScale();
        }
    }

    public override void OnTriggerRelease()
    {
        holdingTrigger = false;
        otherController.StopScaling();
        StopScaling();
//		theCollider.enabled = false;
//		pushIndicator.SetActive( false );
    }

    void StartScaling()
    {
        startControllerDistance = Vector3.Distance( playArea.transform.InverseTransformPoint( transform.position ),
            playArea.transform.InverseTransformPoint( otherController.transform.position ) );
        startScale = playArea.transform.localScale.x;
//		pushIndicator.SetActive( false );
//		theCollider.enabled = false;
//		otherController.pushIndicator.SetActive( false );
//		otherController.theCollider.enabled = false;
		scaleLine.gameObject.SetActive( true );
		SetLine();
        scaling = true;
    }

	void SetLine ()
	{
		linePoints[0] = transform.position;
		linePoints[1] = otherController.transform.position;
		scaleLine.SetPositions( linePoints );
	}

    void UpdateScale()
    {
		float currentControllerDistance = Vector3.Distance( playArea.transform.InverseTransformPoint( transform.position ), 
			playArea.transform.InverseTransformPoint( otherController.transform.position ) );
		float d = startControllerDistance * startScale / (startControllerDistance + scaleSpeed * (currentControllerDistance - startControllerDistance));

        if (d > maxScale)
        {
			SceneLoader[] loaders = GameObject.FindObjectsOfType<SceneLoader>();
			for (int i = 0; i < 10; i++)
			{
				if (i >= loaders.Length)
				{
					break;
				}
				if (loaders[i] != null && loaders[i].gameObject != levelLoader.gameObject)
				{
					Destroy( loaders[i].gameObject );
				}
			}
            levelLoader.Trigger();
        }
        playArea.transform.localScale = Mathf.Clamp( d, minScale, maxScale ) * Vector3.one;

		SetFadeUI( d );
		SetLine();
    }

	void SetFadeUI (float d)
	{
		if (d < startFadeScale)
		{
			ShowObject( fader.gameObject, false );
		}
		else
		{
            if (d < fadeTextScale)
            {
                ShowObject(uiCamera, false);
            }
            else
            {
                if (!uiCamera.activeSelf)
                {
                    uiCamera.transform.position = eyes.position;
                    uiCamera.transform.rotation = eyes.rotation;
                    ShowObject(uiCamera, true);
                }
            }
            if (!uiCamera.activeSelf)
            {
                ShowObject(fader.gameObject, true);
            }
            fader.material.color = new Color( faderColor.r, faderColor.g, faderColor.b, d < startFadeScale ? 0 : (d - startFadeScale) / (maxScale - startFadeScale) );
		}
	}

    void StopScaling()
    {
        scaling = false;
		scaleLine.gameObject.SetActive( false );
//		if (holdingTrigger)
//		{
//			pushIndicator.SetActive( true );
//			theCollider.enabled = true;
//		}
//		if (otherController.holdingTrigger)
//		{
//			otherController.pushIndicator.SetActive( true );
//			theCollider.enabled = true;
//		}
    }

	protected override void DoUpdate ()
	{
		UpdateButtonLabels();
        UpdateTime();
	}

	public override void OnDPadUpEnter () 
	{
		timeUI.SetHover( TimeHoverState.Up );
        currentTimeHover = TimeHoverState.Up;
    }

	public override void OnDPadDownEnter () 
	{
		timeUI.SetHover( TimeHoverState.Down );
        currentTimeHover = TimeHoverState.Down;
	}

	public override void OnDPadRightEnter () 
	{
		timeUI.SetHover( TimeHoverState.None );
        currentTimeHover = TimeHoverState.None;
    }

	public override void OnDPadLeftEnter () 
	{
		timeUI.SetHover( TimeHoverState.None );
        currentTimeHover = TimeHoverState.None;
    }

	public override void OnDPadExit () 
	{
		timeUI.SetHover( TimeHoverState.None );
        currentTimeHover = TimeHoverState.None;
    }

    TimeHoverState currentTimeHover = TimeHoverState.None;
    public bool timeOnPress = true;

    void UpdateTime ()
    {
        if (!timeOnPress)
        {
            if (currentTimeHover == TimeHoverState.Up)
            {
                if (timeUI != null)
                {
                    ChangeTime( 0.8f );
                }
            }
            else if (currentTimeHover == TimeHoverState.Down)
            {
                if (timeUI != null)
                {
                    ChangeTime( 1.2f );
                }
            }
        }
    }

    public override void OnDPadUpStay()
    {
		if (timeUI != null && timeOnPress)
		{
	        ChangeTime( 0.8f );
		}
    }

    public override void OnDPadDownStay()
    {
		if (timeUI != null && timeOnPress)
		{
	        ChangeTime( 1.2f );
		}
    }
    
    void ChangeTime (float delta)
    {
        float timeMultiplier = Mathf.Clamp( MolecularEnvironment.Instance.timeMultiplier * delta, minTimeMultiplier, maxTimeMultiplier );
        MolecularEnvironment.Instance.SetTime( timeMultiplier );
		timeUI.Set( timeMultiplier );
    }

	void UpdateButtonLabels ()
	{
		ShowObject( scaleButtonLabel, idle );
		ShowObject( labelLine, idle );
	}

	void ShowObject (GameObject label, bool show)
	{
		if (show && !label.activeSelf)
		{
			label.SetActive( true );
		}
		else if (!show && label.activeSelf)
		{
			label.SetActive( false );
		}
	}

    void DoReset()
    {
        kinesin.DoReset();
        microtubule.DoReset();
        MolecularEnvironment.Instance.Reset();
    }
}
