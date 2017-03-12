using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelFireFlag : StateMachineBehaviour {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool(AvatarController.FIRE_TRIGGER, false);
    }
}
