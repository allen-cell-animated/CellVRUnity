using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCellAnimation : StateMachineBehaviour 
{
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        animator.transform.GetChild( 0 ).parent = animator.GetComponentInParent<CellAnimator>().oldParent;
        Destroy( animator.transform.parent.gameObject );
	}
}
