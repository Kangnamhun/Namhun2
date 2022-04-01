using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillShot : StateMachineBehaviour
{
    public string boolName;
    public bool status;
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Fire", status);
    }
}
