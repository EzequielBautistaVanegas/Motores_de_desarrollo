using UnityEngine;

public class InteractionAnimState : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Movement movement = animator.GetComponentInParent<Movement>();
        if (movement != null) movement.SetBusy(false);
    }
}