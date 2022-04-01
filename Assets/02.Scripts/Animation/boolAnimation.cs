using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boolAnimation : StateMachineBehaviour
{
    public string boolName;
    public bool status;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(boolName, !status);
    }
}
