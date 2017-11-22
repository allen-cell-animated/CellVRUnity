﻿using UnityEngine;
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
    public float minTimeMultiplier = 1f;
    public float maxTimeMultiplier = 10000f;
    public float scaleMultiplier = 0.1f;
    public float minScale = 15f;
    public float maxScale = 80f;
	public float startFadeScale = 50f;
    public bool holdingTrigger = false;

    bool scaling = false;
    float startControllerDistance;
    float startScale;
	Color faderColor;
	float lastD = 80f;

	void Start ()
	{
		faderColor = fader.material.color;
	}

    public override void OnTriggerPull()
    {
        holdingTrigger = true;
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
    }

    void StartScaling()
    {
        startControllerDistance = Vector3.Distance( playArea.transform.InverseTransformPoint( transform.position ),
            playArea.transform.InverseTransformPoint( otherController.transform.position ) );
        startScale = playArea.transform.localScale.x;
        scaling = true;
    }

    void UpdateScale()
    {
        float d = startControllerDistance / Vector3.Distance( playArea.transform.InverseTransformPoint( transform.position ), 
            playArea.transform.InverseTransformPoint( otherController.transform.position ) ) * startScale;
        if (d > maxScale)
        {
            levelLoader.Trigger();
        }
        playArea.transform.localScale = Mathf.Clamp( d, minScale, maxScale ) * Vector3.one;

		if (d >= lastD)
		{
			fader.material.color = new Color( faderColor.r, faderColor.g, faderColor.b, d < startFadeScale ? 0 : (d - minScale) / (maxScale - minScale) );
		}
		else
		{
			fader.material.color = new Color( faderColor.r, faderColor.g, faderColor.b, 0 );
		}
		lastD = d;
    }

    void StopScaling()
    {
        scaling = false;
    }

    public override void OnDPadUpStay()
    {
        ChangeTime( 0.9f );
    }

    public override void OnDPadDownStay()
    {
        ChangeTime( 1.1f );
    }
    
    void ChangeTime (float delta)
    {
        float timeMultiplier = Mathf.Clamp( MolecularEnvironment.Instance.timeMultiplier * delta, minTimeMultiplier, maxTimeMultiplier );
        MolecularEnvironment.Instance.SetTime( timeMultiplier );
    }

    void DoReset()
    {
        kinesin.DoReset();
        microtubule.DoReset();
        MolecularEnvironment.Instance.Reset();
    }
}
