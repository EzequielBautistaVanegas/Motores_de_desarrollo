using UnityEngine;

public class LeverInteractable : InteractableBase
{
    [SerializeField] private LeverPuzzle puzzle;
    [SerializeField] private Animator animator;
    [SerializeField] private string onParameter = "On";

    public bool IsOn { get; private set; }

    public override void Interact(PlayerInteractor player)
    {
        IsOn = !IsOn;

        if (animator != null)
        {
            animator.SetBool(onParameter, IsOn);
        }

        if (puzzle != null)
        {
            puzzle.CheckSolution();
        }
    }
}
