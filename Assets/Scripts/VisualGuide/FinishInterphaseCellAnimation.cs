using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishInterphaseCellAnimation : StateMachineBehaviour 
{
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        VisualGuideManager.Instance.interphaseCell.ExitIsolationMode();
	}
}
