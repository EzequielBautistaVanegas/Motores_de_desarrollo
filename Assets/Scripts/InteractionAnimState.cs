using UnityEngine;

// Agregar como Behaviour a los estados Open y Close de la capa UpperBody.
// Desbloquea al player cuando termina la animación.
public class InteractionAnimState : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Movement movement = animator.GetComponentInParent<Movement>();
        if (movement != null) movement.SetBusy(false);
    }
}