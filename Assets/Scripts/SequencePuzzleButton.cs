using UnityEngine;

public class SequencePuzzleButton : InteractableBase
{
    [SerializeField] private SequencePuzzle puzzle;
    [SerializeField] private int id;

    public override void Interact(PlayerInteractor player)
    {
        if (puzzle != null)
        {
            puzzle.Press(id);
        }
    }
}
