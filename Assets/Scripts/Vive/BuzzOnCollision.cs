using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(SteamVR_TrackedObject) )]
public class BuzzOnCollision : MonoBehaviour
{
    public ushort pulseLength = 300; 

	int _deviceIndex = -1;
	int deviceIndex
	{
		get {
			if (_deviceIndex < 0)
			{
				_deviceIndex = (int)GetComponent<SteamVR_TrackedObject>().index;
			}
			return _deviceIndex;
		}
    }

    void OnCollisionEnter (Collision collision)
    {
        Buzz();
    }

    void OnTriggerEnter (Collider other)
    {
		AICS.MotorProteins.Kinesin.Motor motor = other.GetComponent<AICS.MotorProteins.Kinesin.Motor>();
		if (motor != null && motor.stateIsStrong)
		{
			Debug.Log( "controller is touching strong motor!" );
		}
        Buzz();
    }

    void Buzz ()
    {
        SteamVR_Controller.Input(deviceIndex).TriggerHapticPulse(pulseLength);
    }
}
