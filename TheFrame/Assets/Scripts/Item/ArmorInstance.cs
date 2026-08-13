using System.Text;
using UnityEngine;

[System.Serializable]
public class ArmorInstance : ItemInstance
{
    public float ReduceMultiplier { get; private set; }
    public float HealthMultiplier { get; private set; }
    public float AmmoMultiplier { get; private set; }
    public float SpeedMultiplier { get; private set; }

    private float plusReduce;
    private float plusHealth;
    private float plusAmmo;
    private float plusSpeed;

    //0 : plate, 1 : pouch
    private AttachmentInstance[] parts = new AttachmentInstance[2];
    public AttachmentInstance[] Parts => parts;
    private StringBuilder sb = new StringBuilder();

    public ArmorInstance() : base() { }

    public ArmorInstance(Armor data) : base(data) { }

    public ArmorInstance(ArmorInstance armorIns) : base(armorIns)
    {
        ReduceMultiplier = armorIns.ReduceMultiplier;
        HealthMultiplier = armorIns.HealthMultiplier;
        AmmoMultiplier = armorIns.AmmoMultiplier;
        SpeedMultiplier = armorIns.SpeedMultiplier;

        for (int i = 0; i < armorIns.parts.Length; i++)
        {
            parts[i] = new AttachmentInstance(armorIns.parts[i]);
        }
    }

    public Armor armor => (Armor)Data;

    public void Init()
    {
        //½ºÅÈ ·£´ı ¹èÀ² ¼³Á¤
        ReduceMultiplier = Random.Range(0.8f, 1.2f);
        HealthMultiplier = Random.Range(0.8f, 1.2f);
        AmmoMultiplier = Random.Range(0.8f, 1.2f);
        SpeedMultiplier = Random.Range(0.8f, 1.2f);

        //ÀåÂøµÈ ÆÄÃ÷ ¼³Á¤
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = new AttachmentInstance();
        }
    }

    public bool CheckParts(int index)
    {
        return parts[index].attachment != null ? true : false;
    }

    public AttachmentInstance InstallParts(AttachmentInstance attachment)
    {
        AttachmentInstance instance = null;

        switch (attachment.attachment.partsType)
        {
            case PartsType.plate:
                if (parts[0].attachment != null)
                    instance = new AttachmentInstance(parts[0]);
                parts[0] = attachment;
                break;
            case PartsType.pouch:
                if (parts[1].attachment != null)
                    instance = new AttachmentInstance(parts[1]);
                parts[1] = attachment;
                break;
        }
        PlusStatSet();
        return instance;
    }

    public AttachmentInstance UnInstallParts(AttachmentInstance attachment)
    {
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] == attachment)
            {
                parts[i] = new AttachmentInstance();
                break;
            }
        }
        PlusStatSet();
        return attachment;
    }

    public void Copy(ArmorInstance armor)
    {
        ReduceMultiplier = armor.ReduceMultiplier;
        HealthMultiplier = armor.HealthMultiplier;
        AmmoMultiplier = armor.AmmoMultiplier;
        SpeedMultiplier = armor.SpeedMultiplier;

        plusReduce = armor.plusReduce;
        plusHealth = armor.plusHealth;
        plusAmmo = armor.plusAmmo;
        plusSpeed = armor.plusSpeed;

        parts = armor.parts;
    }

    public void PlusStatSet()
    {
        plusReduce = 0f;
        plusAmmo = 0f;
        plusSpeed = 0f;

        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].attachment != null)
            {
                for (int j = 0; j < parts[i].attachment.StatusDatas.Length; j++)
                {
                    AttachmentStatusData statusData = parts[i].attachment.StatusDatas[j];
                    switch (statusData.StatusType)
                    {
                        case AttachmentStatusType.extraReduce:
                            plusReduce += statusData.Value;
                            break;
                        case AttachmentStatusType.extraAmmo:
                            plusAmmo += statusData.Value;
                            break;
                        case AttachmentStatusType.extraSpeed:
                            plusSpeed += statusData.Value;
                            break;
                    }
                }
            }
        }
    }

    public float FinalStat(float stat, float Multiplier, float plusStat)
    {
        int s = (int)(stat * Multiplier + plusStat);
        return s;
    }

    public float FinalReduce()
    {
        return FinalStat(armor.DamageReduce, ReduceMultiplier, plusReduce);
    }

    public float FinalHealth()
    {
        return FinalStat(armor.ExtraHealth, HealthMultiplier, plusHealth);
    }

    public float FinalAmmo()
    {
        return FinalStat(armor.MaxAmmo, AmmoMultiplier, plusAmmo);
    }

    public float FinalSpeed()
    {
        return FinalStat(armor.ExtraSpeed, SpeedMultiplier, plusSpeed);
    }

    public string DescStr()
    {
        sb.Clear();

        sb.AppendLine($"Reduce : {FinalReduce()}");
        sb.AppendLine($"Health : {FinalHealth()}");
        sb.AppendLine($"Ammo : {FinalAmmo()}");
        sb.AppendLine($"Speed : {FinalSpeed()}");
        return sb.ToString();
    }

    public override AttachmentInstance[] GetPartsArray()
    {
        return parts;
    }

}
