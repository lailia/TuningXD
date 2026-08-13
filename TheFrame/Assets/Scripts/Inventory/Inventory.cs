using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    [Header("Inventory")]
    public int slotCount;
    public GameObject slotPrefab;
    public Transform InventoryParentTransform;
    public TextMeshProUGUI slotCountText;
    public InventoryCategoryUI categoryUI;

    [Header("Equipment")]
    public Equipment equipment;

    [Header("ItemDataPanel")]
    public InventoryItemDataPanel itemDataPanel;

    [Header("Modify")]
    public ModifyManager modifyManager;

    [Header("밖에서 손대지 않는 변수")]
    public List<InventorySlot> inventorySlotList = new List<InventorySlot>();
    public List<ItemInstance> itemDataList = new List<ItemInstance>();
    public List<WeaponInstance> weaponDataList = new List<WeaponInstance>();
    public List<ArmorInstance> armorDataList = new List<ArmorInstance>();
    public List<AttachmentInstance> attachmentDataList = new List<AttachmentInstance>();
    public ItemType Type { get; private set; }

    [Header("Items")]
    public Weapon[] weaponDatas;
    public Armor[] armorDatas;
    public Attachment[] partsDatas;

    //선택된 아이템 데이터
    public ItemInstance SelectItemData { get; private set; }

    public void Init()
    {
        //인벤토리 슬롯 생성
        SlotInit();
    }

    public void SlotInit()
    {
        if (inventorySlotList.Count < slotCount)
        {
            for (int i = 0; i < slotCount; i++)
            {
                GameObject obj = Instantiate(slotPrefab, InventoryParentTransform);
                InventorySlot s = obj.GetComponent<InventorySlot>();
                s.Init(i, ClickSlot);
                inventorySlotList.Add(s);
            }
        }
    }


    public void ClickSlot(ItemInstance item, bool check)      //슬롯 번호를 받아와서 현재 선택된 인벤토리에서 데이터를 가져옴
    {
        itemDataPanel.PanelActiveFalse();

        if (item != null)
            ItemDataPanelSet(item, check);
    }

    public void ItemDataPanelSet(ItemInstance itemData, bool check)
    {
        if (itemData.Data != null)
        {
            SelectItemData = itemData;
            itemDataPanel.ItemDataUpdate(itemData, check);
        }
    }

    #region Inventory

    //랜덤 아이템 생성
    public void AddRandomItem()
    {
        // 랜덤한 아이템 나오게하는 방법 찾기
        int randType = Random.Range(0, 3);

        if (itemDataList.Count < slotCount)
        {
            switch (randType)
            {
                case 0:
                    int rand = Random.Range(0, weaponDatas.Length);
                    WeaponInstance weapon = new WeaponInstance(weaponDatas[rand]);
                    weapon.Init();
                    AddItem(weapon);
                    break;
                case 1:
                    rand = Random.Range(0, armorDatas.Length);
                    ArmorInstance armor = new ArmorInstance(armorDatas[rand]);
                    armor.Init();
                    AddItem(armor);
                    break;
                case 2:
                    rand = Random.Range(0, partsDatas.Length);
                    AttachmentInstance parts = new AttachmentInstance(partsDatas[rand]);
                    parts.Init();
                    AddItem(parts);
                    break;
            }

            SlotUpdate(Type);
        }
        
    }

    public void AddItem(ItemInstance item)
    {
        itemDataList.Add(item);
        if(item.Data.itemType==ItemType.weapon)
        {
            weaponDataList.Add(item as WeaponInstance);
        }
        else if(item.Data.itemType==ItemType.armor)
        {
            armorDataList.Add(item as ArmorInstance);
        }
        else if(item.Data.itemType==ItemType.attachment)
        {
            attachmentDataList.Add(item as AttachmentInstance);
        }
    }

    //개조 확정시 사용할 수 있을거 같음
    public void DataListUpdate(bool isModify)
    {
        if (isModify)
        {
            itemDataList.Clear();

            for (int i = 0; i < weaponDataList.Count; i++)
            {
                itemDataList.Add(weaponDataList[i]);
            }

            for (int i = 0; i < armorDataList.Count; i++)
            {
                itemDataList.Add(armorDataList[i]);
            }

            for (int i = 0; i < attachmentDataList.Count; i++)
            {
                itemDataList.Add(attachmentDataList[i]);
            }
        }
        else
        {
            weaponDataList.Clear();
            armorDataList.Clear();
            attachmentDataList.Clear();

            for (int i = 0; i < itemDataList.Count; i++)
            {
                if (itemDataList[i] is WeaponInstance weapon)
                {
                    weaponDataList.Add(weapon);
                }
                else if (itemDataList[i] is ArmorInstance armor)
                {
                    armorDataList.Add(armor);
                }
                else if (itemDataList[i] is AttachmentInstance attachment)
                {
                    attachmentDataList.Add(attachment);
                }
            }
        }
    }

    //인벤토리 슬롯 업데이트
    public void SlotUpdate(ItemType type)
    {
        for (int i = 0; i < inventorySlotList.Count; i++)
        {
            inventorySlotList[i].ResetSlot();
        }

        InvenSort();

        if (type == ItemType.all)
        {
            for (int i = 0; i < itemDataList.Count; i++)
            {
                inventorySlotList[i].SetItem(itemDataList[i]);
            }
        }
        else if (type == ItemType.weapon)
        {
            for (int i = 0; i < weaponDataList.Count; i++)
            {
                inventorySlotList[i].SetItem(weaponDataList[i]);
            }
        }
        else if (type == ItemType.armor)
        {
            for (int i = 0; i < armorDataList.Count; i++)
            {
                inventorySlotList[i].SetItem(armorDataList[i]);
            }
        }
        else if (type == ItemType.attachment)
        {
            for (int i = 0; i < attachmentDataList.Count; i++)
            {
                inventorySlotList[i].SetItem(attachmentDataList[i]);
            }
        }

        SlotCountCheck();
    }

    public void CategoryButtonClick(int index)
    {
        Type = (ItemType)index;
        SlotUpdate(Type);
        categoryUI.HandleSlotClick(index);
    }

    //인벤토리 열기 및 닫기
    public void InventoryUIActive()
    {
        if (gameObject.activeInHierarchy)
        {
            itemDataPanel.PanelActiveFalse(); 
            gameObject.SetActive(false);
        }
        else
        {
            Init();
            SlotUpdate(Type);
            gameObject.SetActive(true);
        }
    }

    //인벤토리 최대 슬롯과 현재 사용중인 슬롯 수
    public void SlotCountCheck()
    {
        slotCountText.text = $"{itemDataList.Count}/{slotCount}";
    }

    public void RemoveItemClick()
    {
        RemoveItem(SelectItemData);
        itemDataPanel.PanelActiveFalse();
    }

    //아이템 삭제
    public void RemoveItem(ItemInstance item)
    {
        if (item is WeaponInstance weapon)
        {
            weaponDataList.Remove(weapon);
        }
        else if (item is ArmorInstance armor)
        {
            armorDataList.Remove(armor);
        }
        else if (item is AttachmentInstance attachment)
        {
            attachmentDataList.Remove(attachment);
        }
        itemDataList.Remove(item);
        SlotUpdate(Type);
    }

    public void InvenSort()
    {
        itemDataList.Sort((a, b) => a.Data.itemId.CompareTo(b.Data.itemId));
        weaponDataList.Sort((a, b) => a.weapon.itemId.CompareTo(b.weapon.itemId));
        armorDataList.Sort((a, b) => a.armor.itemId.CompareTo(b.armor.itemId));
        attachmentDataList.Sort((a, b) => a.attachment.itemId.CompareTo(b.attachment.itemId));
    }
    #endregion
    //-------------------------------------------------------------------------------

    #region Equip
    //인벤토리 장비 장착
    public void OnEquipItem()
    {
        equipment.SetItem(SelectItemData);
        RemoveItem(SelectItemData);
        ItemInstance changeItem = equipment.EquipChange();
        if (changeItem != null)                             
        {
            AddItem(changeItem);
        }
        itemDataPanel.PanelActiveFalse();
        SlotUpdate(Type);
    }

    //장착 장비 해제
    public void UnEquipItem()
    {
        if (itemDataList.Count < 25)
        {
            equipment.UnEquipItem(SelectItemData);
            AddItem(SelectItemData);

            SlotUpdate(Type);

            itemDataPanel.PanelActiveFalse();
        }
    }

    public void EquipItemClick(int index)
    {
        ItemDataPanelSet(equipment.GetItem(index), true);
    }

    #endregion
    //-------------------------------------------------------------------------------

    #region ModiFi
    public void ModifyButtonClick()
    {
        modifyManager.Init(attachmentDataList, SelectItemData);
        itemDataPanel.PanelActiveFalse();
    }

    public void ModifyComplete()
    {
        DataListUpdate(modifyManager.ModifyComplete());
        SlotUpdate(Type);
    }

    public void ModifyCancel()
    {
        DataListUpdate(modifyManager.ModifyCancel());
        SlotUpdate(Type);
    }
    #endregion
    //-------------------------------------------------------------------------------

    #region Event
    //키보드 누를 시 랜덤 아이템 생성
    public void OnItemInstantiate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            AddRandomItem();
        }
    }

    //키보드 누를 시 인벤토리 열기 및 닫기
    public void OnInventoryOnOff(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            InventoryUIActive();
        }
    }

    public void OnSortInventory(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            SlotUpdate(Type);
        }
    }
    #endregion
    //-------------------------------------------------------------------------------
}
