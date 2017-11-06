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
    public float minTimeMultiplier = 1f;
    public float maxTimeMultiplier = 10000f;

    public override void OnDPadUpEnter()
    {

    }

    public override void OnDPadDownEnter()
    {

    }

    public override void OnDPadExit()
    {

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

    public override void OnTriggerPull()
    {
        DoReset();
    }

    void DoReset()
    {
        kinesin.Reset();
        microtubule.DoReset();
        MolecularEnvironment.Instance.Reset();
    }
}
