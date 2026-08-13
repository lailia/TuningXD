using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModifySelectPanel : MonoBehaviour 
{
    public ItemInstance itemData;
    public ItemInstance copyItemData;
    public Image itemImage;
    public AttachmentSlot[] attachmentSlots;

    public Sprite[] weaponPartsSprite;
    public Sprite[] armorPartsSprite;

    public TextMeshProUGUI beforeText;
    public TextMeshProUGUI afterText;

    void Start()
    {

    }

    void Update()
    {

    }

    public void Init(ItemInstance item, Action<AttachmentInstance, bool> clickSlot)
    {
        itemData = item;
        if (item is WeaponInstance weapon)
        {
            copyItemData = new WeaponInstance(weapon);
        }
        else if(item is ArmorInstance armor)
        {
            copyItemData = new ArmorInstance(armor);
        }
        itemImage.sprite = itemData.Data.icon;
        SlotsInit(clickSlot);
        BeforeTextUpdate();
    }

    public void SlotsInit(Action<AttachmentInstance, bool> clickSlot)
    {
        for (int i = 0; i < attachmentSlots.Length; i++)
        {
            attachmentSlots[i].gameObject.SetActive(false);
        }

        //이걸 인터페이스로 할지 상속으로 할지 아니면 다른 방법이 있는지 찾아보기
        AttachmentInstance[] parts = itemData.GetPartsArray();

        for (int i = 0; i < parts.Length; i++)
        {
            attachmentSlots[i].Init(weaponPartsSprite[i], i, clickSlot);
            if (parts[i].attachment != null)
            {
                attachmentSlots[i].NewImageChange(parts[i]);
            }
            else
            {
                attachmentSlots[i].BaseImageChange();
            }
            attachmentSlots[i].gameObject.SetActive(true);
        }
    }

    public void SlotUpdate()
    {
        AttachmentInstance[] parts = itemData.GetPartsArray();
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].attachment != null)
                attachmentSlots[i].NewImageChange(parts[i]);
            else
                attachmentSlots[i].BaseImageChange();
        }
    }

    /// <summary>
    /// 반환 값 = 이전에 사용하던 부착물
    /// </summary>
    /// <param name="attachment"></param>
    /// <returns></returns>
    public AttachmentInstance InstallAttachment(AttachmentInstance attachment)
    {
        AttachmentInstance attach = null;
        if(itemData is WeaponInstance weapon)
        {
            attach = weapon.InstallParts(attachment);
        }
        else if(itemData is ArmorInstance armor)
        {
            attach = armor.InstallParts(attachment);
        }

        SlotUpdate();
        AfterTextUpdate();
        return attach;
    }

    public AttachmentInstance UnInstallAttachment(AttachmentInstance attachment)
    {
        AttachmentInstance attach = null;
        if (itemData is WeaponInstance weapon)
        {
            attach = weapon.UnInstallParts(attachment);
        }
        else if (itemData is ArmorInstance armor)
        {
            attach = armor.UnInstallParts(attachment);
        }

        SlotUpdate();
        AfterTextUpdate();
        return attach;
    }

    public void BeforeTextUpdate()
    {
        if(copyItemData is WeaponInstance weapon)
        {
            beforeText.text = weapon.DescStr();
        }
        else if(copyItemData is ArmorInstance armor)
        {
            beforeText.text = armor.DescStr();
        }
    }

    public void AfterTextUpdate()
    {
        if (itemData is WeaponInstance weapon)
        {
            afterText.text = weapon.DescStr();
        }
        else if (itemData is ArmorInstance armor)
        {
            afterText.text = armor.DescStr();
        }
    }

    public void ModifyCancel()
    {
        if(itemData is WeaponInstance weapon)
        {
            weapon.Copy(copyItemData as WeaponInstance);
        }
        else if(itemData is ArmorInstance armor)
        {
            armor.Copy(copyItemData as ArmorInstance);
        }
    }
}
