using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCellAnimation : StateMachineBehaviour 
{
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        VisualGuideManager.Instance.CheckSucess();
	}
}
