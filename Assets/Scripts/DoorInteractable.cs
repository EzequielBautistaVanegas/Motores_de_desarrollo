using UnityEngine;

public class DoorInteractable : InteractableBase
{
    [SerializeField] private Animator animator;
    [SerializeField] private string openParameter = "Open";

    private bool isOpen;

    public override void Interact(PlayerInteractor player)
    {
        isOpen = !isOpen;

        if (animator != null)
        {
            animator.SetBool(openParameter, isOpen);
        }
    }
}
