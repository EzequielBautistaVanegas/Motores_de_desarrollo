using UnityEngine;

public class LockedDoor : InteractableBase
{
    [SerializeField] private Animator animator;
    [SerializeField] private string openParameter = "Open";

    [SerializeField] private ItemData requiredKey;
    [SerializeField] private bool consumeKey;

    private bool opened;

    public override void Interact(PlayerInteractor player)
    {
        if (opened || player == null || player.Inventory == null)
        {
            return;
        }

        if (requiredKey != null && !player.Inventory.HasItem(requiredKey))
        {
            Debug.Log("La puerta esta cerrada");
            return;
        }

        if (requiredKey != null && consumeKey)
        {
            player.Inventory.RemoveItem(requiredKey, 1);
        }

        opened = true;

        if (animator != null)
        {
            animator.SetBool(openParameter, true);
        }
    }
}
