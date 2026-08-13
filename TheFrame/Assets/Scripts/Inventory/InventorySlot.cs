using NUnit.Framework.Interfaces;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlot : MonoBehaviour
{
    public Button button;
    public Sprite defalutImage;
    public Image imageComponent;
    public ItemInstance Item { get; private set; }
    public int slotIndex;

    void Start()
    {

    }

    //개조창 용
    public void Init(int index, Action<AttachmentInstance, bool> clickSlot)
    {
        slotIndex = index;
        button.onClick.AddListener(() => clickSlot(Item as AttachmentInstance, false));
    }

    //인벤토리 용
    public void Init(int index, Action<ItemInstance, bool> clickSlot)
    {
        slotIndex = index;
        button.onClick.AddListener(() => clickSlot(Item, false));
    }

    public void SetItem(ItemInstance itemData)
    {
        Item = itemData;
        ImageUpdate(itemData.Data.icon, Color.white);
    }

    public void ResetSlot()
    {
        Item = null;
        ImageUpdate(defalutImage, Color.black);
    }

    public void ImageUpdate(Sprite sprite, Color color)
    {
        imageComponent.sprite = sprite;
        imageComponent.color = color;
    }
}
