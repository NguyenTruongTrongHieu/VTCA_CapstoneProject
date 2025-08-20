using UnityEngine;

public class Item
{
    public ItemLevel itemLevel;
    public ItemType itemType;

    public Item(ItemLevel itemLevel, ItemType itemType)
    {
        this.itemLevel = itemLevel;
        this.itemType = itemType;
    }
}
