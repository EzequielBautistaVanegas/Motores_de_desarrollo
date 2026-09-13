using UnityEngine;

public class PickupItem : InteractableBase
{
    [SerializeField] private ItemData item;
    [SerializeField, Min(1)] private int amount = 1;

    public override void Interact(PlayerInteractor player)
    {
        if (player == null || player.Inventory == null || item == null)
        {
            return;
        }

        if (player.Inventory.AddItem(item, amount))
        {
            Debug.Log($"Picked up: {item.itemName} x{amount}");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventory full.");
        }
    }
}
