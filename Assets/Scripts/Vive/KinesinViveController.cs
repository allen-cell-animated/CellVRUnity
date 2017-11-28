using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AICS.MotorProteins;
using AICS.MotorProteins.Kinesin;
using AICS.MT;

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
	public GameObject pushButtonLabel;
	public GameObject scaleButtonLabel;
	public LineRenderer scaleLine;
	public GameObject pushIndicator;
	public GameObject labelLine;

    bool scaling = false;
    float startControllerDistance;
    float startScale;
	Color faderColor;
	Collider theCollider;
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
		theCollider = GetComponent<Collider>();
		theCollider.enabled = false;
		scaleLine.gameObject.SetActive( false );
		pushIndicator.SetActive( false );
	}

    public override void OnTriggerPull()
    {
        holdingTrigger = true;
		pushIndicator.SetActive( true );
        if (otherController.holdingTrigger)
        {
            StartScaling();
        }
		theCollider.enabled = true;
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
		theCollider.enabled = false;
		pushIndicator.SetActive( false );
    }

    void StartScaling()
    {
        startControllerDistance = Vector3.Distance( playArea.transform.InverseTransformPoint( transform.position ),
            playArea.transform.InverseTransformPoint( otherController.transform.position ) );
        startScale = playArea.transform.localScale.x;
		pushIndicator.SetActive( false );
		theCollider.enabled = false;
		otherController.pushIndicator.SetActive( false );
		otherController.theCollider.enabled = false;
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
            levelLoader.Trigger();
        }
        playArea.transform.localScale = Mathf.Clamp( d, minScale, maxScale ) * Vector3.one;

		SetFadeUI( d );
		SetLine();
    }

	void SetFadeUI (float d)
	{
		fader.material.color = new Color( faderColor.r, faderColor.g, faderColor.b, d < startFadeScale ? 0 : (d - startFadeScale) / (maxScale - startFadeScale) );

		if (d < fadeTextScale && fadeText.activeSelf)
		{
			fadeText.SetActive( false );
		}
		else if (d >= fadeTextScale && !fadeText.activeSelf)
		{
			fadeText.transform.position = Camera.main.transform.position + 300f * Camera.main.transform.forward;
            fadeText.transform.rotation = Quaternion.LookRotation( fadeText.transform.position - Camera.main.transform.position );
            fadeText.SetActive( true );
		}
	}

    void StopScaling()
    {
        scaling = false;
		scaleLine.gameObject.SetActive( false );
		if (holdingTrigger)
		{
			pushIndicator.SetActive( true );
			theCollider.enabled = true;
		}
		if (otherController.holdingTrigger)
		{
			otherController.pushIndicator.SetActive( true );
			theCollider.enabled = true;
		}
    }

	protected override void DoUpdate ()
	{
		UpdateButtonLabels();
	}

	public override void OnDPadUpEnter () 
	{
		timeUI.SetHover( TimeHoverState.Up );
	}

	public override void OnDPadDownEnter () 
	{
		timeUI.SetHover( TimeHoverState.Down );
	}

	public override void OnDPadRightEnter () 
	{
		timeUI.SetHover( TimeHoverState.None );
	}

	public override void OnDPadLeftEnter () 
	{
		timeUI.SetHover( TimeHoverState.None );
	}

	public override void OnDPadExit () 
	{
		timeUI.SetHover( TimeHoverState.None );
	}

    public override void OnDPadUpStay()
    {
		if (timeUI != null)
		{
	        ChangeTime( 0.9f );
		}
    }

    public override void OnDPadDownStay()
    {
		if (timeUI != null)
		{
	        ChangeTime( 1.1f );
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
		if (idle && otherController.idle)
		{
			ShowLabel( pushButtonLabel, true );
			ShowLabel( scaleButtonLabel, false );
			ShowLabel( labelLine, true );
		}
		else if (idle && otherController.holdingTrigger)
		{
			ShowLabel( pushButtonLabel, false );
			ShowLabel( scaleButtonLabel, true );
			ShowLabel( labelLine, true );
		}
		else
		{
			ShowLabel( pushButtonLabel, false );
			ShowLabel( scaleButtonLabel, false );
			ShowLabel( labelLine, false );
		}
	}

	void ShowLabel (GameObject label, bool show)
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
