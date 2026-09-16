using UnityEngine;

public enum ItemType
{
    Key,
    Oil,
    PuzzleItem,
    Other
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    [Header("Item")]
    public string itemId;
    public string itemName;
    public Sprite icon;

    [Header("Inventario")]
    public ItemType itemType;
    [Min(1)] public int maxStack = 1;

    [Header("Consumible")]
    [Min(0f)] public float useValue;
}
