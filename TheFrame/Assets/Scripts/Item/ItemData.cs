using UnityEngine;

public enum ItemType
{
    all,
    weapon,
    armor,
    attachment
}

public class ItemData : ScriptableObject
{
    public int itemId;
    public string itemName;
    public Sprite icon;

    public ItemType itemType;
}
