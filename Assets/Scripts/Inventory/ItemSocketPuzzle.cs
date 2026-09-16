using UnityEngine;
using UnityEngine.Events;

public class ItemSocketPuzzle : InteractableBase
{
    [SerializeField] private ItemData requiredItem;
    [SerializeField] private bool consumeItem = true;
    [SerializeField] private UnityEvent onSolved;

    private bool solved;

    public bool IsSolved => solved;

    public override void Interact(PlayerInteractor player)
    {
        if (solved || player == null || player.Inventory == null || requiredItem == null)
        {
            return;
        }

        Inventory inventory = player.Inventory;

        if (!inventory.HasItem(requiredItem))
        {
            Debug.Log($"Missing required item: {requiredItem.itemName}");
            return;
        }

        if (consumeItem)
        {
            inventory.RemoveItem(requiredItem, 1);
        }

        solved = true;
        Debug.Log("Puzzle completado");
        onSolved?.Invoke();
    }
}
