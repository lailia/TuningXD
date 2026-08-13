using System.Collections.Generic;
using UnityEngine;

public class ModifyManager : MonoBehaviour
{
    public ModifyInventory mInven;
    public ModifySelectPanel mSelectPanel;
    public ModifyItemDataPanel mDataPanel;

    [SerializeField]
    private AttachmentInstance selectedAttachment;

    public void Init(List<AttachmentInstance> inven, ItemInstance item)
    {
        AttachmentType aType = new();
        if (item.Data.itemType == ItemType.weapon)
            aType = AttachmentType.weaponType;
        else if (item.Data.itemType == ItemType.armor)
            aType = AttachmentType.armorType;

        mInven.Init(inven, aType, ClickSlot);   //람다식을 직접 넣는 방법? 
        mSelectPanel.Init(item, ClickSlot);     //람다식을 넣을려면 slot의 변수를 매개변수로 쓰는게 아닌 
        ActiveTrue();                         //여기 클래스의 변수나 매개변수를 써야함
    }

    public void ClickSlot(AttachmentInstance instance, bool isEquiped)
    {
        mDataPanel.PanelActiveFalse();

        if(instance!=null)
        {
            selectedAttachment = instance;
            mDataPanel.DataUpdate(selectedAttachment, isEquiped);
        }
    }

    public void InstallAttachment()
    {
        // attachment : 이전에 사용하던 부착물
        AttachmentInstance attachment = mSelectPanel.InstallAttachment(selectedAttachment);
        mInven.RemoveItem(selectedAttachment);
        if(attachment != null)
        {
            mInven.AddItem(attachment);
        }
        mInven.SlotUpdate();
        mDataPanel.PanelActiveFalse();
    }

    public void UnInstallAttachment()
    {
        AttachmentInstance attachment = mSelectPanel.UnInstallAttachment(selectedAttachment);
        mInven.AddItem(attachment);
        mInven.SlotUpdate();
        mDataPanel.PanelActiveFalse();
    }

    public bool ModifyComplete()
    {
        ActiveFalse();

        return true;
    }

    public bool ModifyCancel()
    {
        mSelectPanel.ModifyCancel();
        ActiveFalse();

        return false;
    }

    public void ActiveTrue()
    {
        gameObject.SetActive(true);
    }

    public void ActiveFalse()
    {
        gameObject.SetActive(false);
    }

}
