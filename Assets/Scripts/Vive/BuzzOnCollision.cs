using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzOnCollision : MonoBehaviour
{
    public int controllerIndex = 0;
    public ushort pulseLength = 300; 

    void OnCollisionEnter(Collision collision)
    {
        SteamVR_Controller.Input( controllerIndex ).TriggerHapticPulse( pulseLength );
    }
}
