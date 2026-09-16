using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int amount;

    public InventorySlot(ItemData item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}

public class Inventory : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxSlots = 8;
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private PlayerLight playerLight;
    [SerializeField] private List<InventorySlot> slots = new();

    public int SelectedIndex { get; private set; }

    public ItemData SelectedItem
    {
        get
        {
            if (slots.Count == 0 || SelectedIndex < 0 || SelectedIndex >= slots.Count)
            {
                return null;
            }

            return slots[SelectedIndex].item;
        }
    }

    public IReadOnlyList<InventorySlot> Slots => slots;

    private void OnEnable()
    {
        if (input == null)
        {
            return;
        }

        input.NextItemPressed += SelectNextItem;
        input.UseItemPressed += UseSelectedItem;
    }

    private void OnDisable()
    {
        if (input == null)
        {
            return;
        }

        input.NextItemPressed -= SelectNextItem;
        input.UseItemPressed -= UseSelectedItem;
    }

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0)
        {
            return false;
        }

        int availableCapacity = 0;

        foreach (InventorySlot slot in slots)
        {
            if (slot.item == item)
            {
                availableCapacity += Mathf.Max(0, item.maxStack - slot.amount);
            }
        }

        availableCapacity += Mathf.Max(0, maxSlots - slots.Count) * item.maxStack;

        if (availableCapacity < amount)
        {
            return false;
        }

        int remaining = amount;

        foreach (InventorySlot slot in slots)
        {
            if (slot.item != item || slot.amount >= item.maxStack)
            {
                continue;
            }

            int freeSpace = item.maxStack - slot.amount;
            int toAdd = Mathf.Min(freeSpace, remaining);

            slot.amount += toAdd;
            remaining -= toAdd;

            if (remaining <= 0)
            {
                return true;
            }
        }

        while (remaining > 0)
        {
            int toAdd = Mathf.Min(item.maxStack, remaining);
            slots.Add(new InventorySlot(item, toAdd));
            remaining -= toAdd;
        }

        return true;
    }

    public bool HasItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0)
        {
            return false;
        }

        int total = 0;

        foreach (InventorySlot slot in slots)
        {
            if (slot.item == item)
            {
                total += slot.amount;
            }
        }

        return total >= amount;
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (!HasItem(item, amount))
        {
            return false;
        }

        int remaining = amount;

        for (int i = slots.Count - 1; i >= 0 && remaining > 0; i--)
        {
            InventorySlot slot = slots[i];

            if (slot.item != item)
            {
                continue;
            }

            int toRemove = Mathf.Min(slot.amount, remaining);
            slot.amount -= toRemove;
            remaining -= toRemove;

            if (slot.amount <= 0)
            {
                slots.RemoveAt(i);
            }
        }

        ClampSelection();
        return true;
    }

    private void SelectNextItem()
    {
        if (slots.Count == 0)
        {
            SelectedIndex = 0;
            return;
        }

        SelectedIndex = (SelectedIndex + 1) % slots.Count;
        Debug.Log($"Selected: {slots[SelectedIndex].item.itemName}");
    }

    private void UseSelectedItem()
    {
        ItemData item = SelectedItem;

        if (item == null)
        {
            return;
        }

        switch (item.itemType)
        {
            case ItemType.Oil:
                if (playerLight != null && playerLight.AddOil(item.useValue))
                {
                    RemoveItem(item, 1);
                }
                break;

            case ItemType.Key:
                Debug.Log("Usa la llave en la cerradura correcta");
                break;

            case ItemType.PuzzleItem:
                Debug.Log("Usa el objeto en el puzzle correcto");
                break;

            case ItemType.Other:
                Debug.Log("Este item no tiene uso directo");
                break;
        }
    }

    private void ClampSelection()
    {
        if (slots.Count == 0)
        {
            SelectedIndex = 0;
            return;
        }

        SelectedIndex = Mathf.Clamp(SelectedIndex, 0, slots.Count - 1);
    }
}
