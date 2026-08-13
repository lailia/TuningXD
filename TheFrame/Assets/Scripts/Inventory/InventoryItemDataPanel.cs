using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemDataPanel : MonoBehaviour
{
    public GameObject itemDataPanel;
    public Image[] attachmentSlots;
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemTypeText;
    public TextMeshProUGUI itemDescText;

    [Header("Buttons")]
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject modifiButton;
    public GameObject removeButton;

    public bool isEquiped;    //1 : 인벤, 2 : 장비창

    void Start()
    {
        itemDataPanel.SetActive(false);
    }

    void Update()
    {

    }

    public void ItemDataPanelSet(ItemInstance item)
    {
        switch (item.Data.itemType)
        {
            case ItemType.weapon:
                WeaponInstance weapon = item as WeaponInstance;
                itemIcon.sprite = weapon.weapon.icon;
                itemNameText.text = weapon.weapon.itemName;
                itemTypeText.text = weapon.weapon.weaponType.ToString();
                itemDescText.text = weapon.DescStr();
                break;
            case ItemType.armor:
                ArmorInstance armor = item as ArmorInstance;
                itemIcon.sprite = armor.armor.icon;
                itemNameText.text = armor.armor.itemName;
                itemTypeText.text = armor.armor.armorType.ToString();
                itemDescText.text = armor.DescStr();
                break;
            case ItemType.attachment:
                AttachmentInstance parts = item as AttachmentInstance;
                itemIcon.sprite = parts.attachment.icon;
                itemNameText.text = parts.attachment.itemName;
                itemTypeText.text = parts.attachment.partsType.ToString();
                itemDescText.text = parts.DescStr();
                break;
        }
    }

    public void ItemDataUpdate(ItemInstance _itemData, bool _check)
    {
        isEquiped = _check;

        ItemDataPanelSet(_itemData);
        AttachmentSet(_itemData);
        ButtonSet(_itemData);

        PanelActiveTrue();
    }

    public void AttachmentSet(ItemInstance itemData)
    {
        for(int i=0;i<attachmentSlots.Length;i++)
        {
            attachmentSlots[i].transform.parent.gameObject.SetActive(false);
        }

        if(itemData.Data.itemType==ItemType.weapon)
        {
            WeaponInstance weapon = itemData as WeaponInstance;
            for(int i=0;i<weapon.Parts.Length;i++)
            {
                attachmentSlots[i].transform.parent.gameObject.SetActive(true);
                if (weapon.CheckParts(i))
                {
                    attachmentSlots[i].sprite = weapon.Parts[i].attachment.icon;
                    attachmentSlots[i].color = Color.white;
                }
                else
                {
                    attachmentSlots[i].sprite = null;
                    attachmentSlots[i].color = Color.black;
                }
            }
        }
        else if(itemData.Data.itemType==ItemType.armor)
        {
            ArmorInstance armor = itemData as ArmorInstance;
            for(int i=0;i<armor.Parts.Length;i++)
            {
                attachmentSlots[i].transform.parent.gameObject.SetActive(true);
                if (armor.CheckParts(i))
                {
                    attachmentSlots[i].sprite = armor.Parts[i].attachment.icon;
                    attachmentSlots[i].color = Color.white;
                }
                else
                {
                    attachmentSlots[i].sprite = null;
                    attachmentSlots[i].color = Color.black;
                }
            }
        }
    }

    public void ButtonSet(ItemInstance itemData)
    {
        equipButton.SetActive(!isEquiped);
        removeButton.SetActive(!isEquiped);
        unEquipButton.SetActive(isEquiped);

        if (itemData.Data.itemType == ItemType.attachment)
            modifiButton.SetActive(false);
        else
            modifiButton.SetActive(true);
    }

    public void PanelActiveTrue()
    {
        itemDataPanel.SetActive(true);
    }

    public void PanelActiveFalse()
    {
        itemDataPanel.SetActive(false);
    }

}
