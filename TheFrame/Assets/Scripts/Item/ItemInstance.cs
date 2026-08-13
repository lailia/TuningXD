using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    [SerializeField]
    private ItemData data;
    public ItemData Data => data;

    public ItemInstance() { }

    public ItemInstance(ItemInstance data)
    {
        this.data = data.Data;
    }

    public ItemInstance(ItemData data)
    {
        this.data = data;
    }

    public virtual AttachmentInstance[] GetPartsArray() { return null; }
}
