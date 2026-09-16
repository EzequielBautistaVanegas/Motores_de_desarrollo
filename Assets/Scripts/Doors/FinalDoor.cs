using UnityEngine;
using UnityEngine.Events;

public class FinalDoor : InteractableBase
{
    [SerializeField] private ItemData[] requiredKeys;
    [SerializeField] private bool consumeKeys;
    [SerializeField] private UnityEvent onVictory;

    private bool opened;

    public override void Interact(PlayerInteractor player)
    {
        if (opened || player == null || player.Inventory == null)
        {
            return;
        }

        if (requiredKeys == null || requiredKeys.Length == 0)
        {
            Debug.LogWarning("FinalDoor no tiene llave asignada");
            return;
        }

        foreach (ItemData key in requiredKeys)
        {
            if (key == null || !player.Inventory.HasItem(key))
            {
                Debug.Log("Necesitas mas llaves");
                return;
            }
        }

        if (consumeKeys)
        {
            foreach (ItemData key in requiredKeys)
            {
                player.Inventory.RemoveItem(key, 1);
            }
        }

        opened = true;
        Debug.Log("Victoria: todas las llaves obtenidas");
        onVictory?.Invoke();
    }
}
