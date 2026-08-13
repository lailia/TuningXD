using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ModifyInventory : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform parentTransform;

    public List<InventorySlot> slots;
    public List<AttachmentInstance> inventory;

    public AttachmentType Type { get; private set; }

    public void Init(List<AttachmentInstance> inven, AttachmentType type, Action<AttachmentInstance,bool> clickSlot)
    {
        Type = type;

        inventory = inven;

        if (slots.Count < 1)
        {
            for (int i = 0; i < 25; i++)
            {
                GameObject slotObject = Instantiate(slotPrefab, parentTransform);
                InventorySlot slotComp = slotObject.GetComponent<InventorySlot>();
                slotComp.Init(i, clickSlot);
                slots.Add(slotComp);
            }
        }
        SlotUpdate();
    }

    public void SlotUpdate()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].ResetSlot();
        }

        int j = 0;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].attachment.attachmentType == Type)
            {
                slots[j].SetItem(inventory[i]);
                j++;
            }
        }
    }

    public void RemoveItem(AttachmentInstance instance)
    {
        inventory.Remove(instance);
    }

    public void AddItem(AttachmentInstance itemData)
    {
        inventory.Add(itemData);
    }

}
