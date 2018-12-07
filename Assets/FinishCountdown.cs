using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCountdown : StateMachineBehaviour 
{
	override public void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        animator.GetComponentInParent<CountdownCanvas>().FinishCountdown();
	}
}
