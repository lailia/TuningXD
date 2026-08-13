using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModifyItemDataPanel : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemTypeText;
    public TextMeshProUGUI itemDescText;

    public GameObject EquipButton;
    public GameObject UnEquipButton;

    public ItemInstance itemData;
    public bool isEquipped;

    void Start()
    {
        PanelActiveFalse();
    }

    public void PanelActive()
    {
        gameObject.SetActive(gameObject.activeSelf ? false : true);
    }

    public void PanelActiveFalse()
    {
        gameObject.SetActive(false);
    }

    public void PanelSet()
    {
        AttachmentInstance parts = itemData as AttachmentInstance;
        itemImage.sprite = parts.attachment.icon;
        itemNameText.text = parts.attachment.itemName;
        itemTypeText.text = parts.attachment.partsType.ToString();
        itemDescText.text = parts.DescStr();
    }

    public void ButtonSet()
    {
        EquipButton.SetActive(!isEquipped);
        UnEquipButton.SetActive(isEquipped);
    }

    public void DataUpdate(ItemInstance _itemData, bool _isEquipped)
    {
        itemData = _itemData;
        isEquipped = _isEquipped;

        ButtonSet();
        PanelSet();
        PanelActive();
    }
}
