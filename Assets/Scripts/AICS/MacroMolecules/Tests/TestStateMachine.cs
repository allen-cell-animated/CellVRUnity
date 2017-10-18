using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AICS.MacroMolecules;

public class TestStateMachine : MonoBehaviour 
{
	public StateMachine stateMachine;
	public int goalStateID;

	void Update () 
	{
		if (stateMachine.currentState.id == goalStateID)
		{
			IntegrationTest.Pass();
		}
	}
}
