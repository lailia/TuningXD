using System.Text;
using UnityEngine;

public enum AttachmentType
{
    weaponType,
    armorType
}

public enum PartsType
{
    muzzle,
    magazine,
    scope,
    stock,
    plate,
    pouch
}

[CreateAssetMenu(fileName = "Attachment", menuName = "ScriptableObjects/Attachment")]
public class Attachment : ItemData
{
    public AttachmentType attachmentType;
    public PartsType partsType;

    public AttachmentStatusData[] StatusDatas;
}
