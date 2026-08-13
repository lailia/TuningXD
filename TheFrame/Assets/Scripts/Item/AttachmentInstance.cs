using System.Text;
using UnityEngine;

[System.Serializable]
public class AttachmentInstance : ItemInstance
{
    private float[] statMultiplier;

    public StringBuilder sb = new StringBuilder();

    public AttachmentInstance() { }

    public AttachmentInstance(Attachment data) : base(data) { }

    public AttachmentInstance(AttachmentInstance instance) : base(instance)
    {
        if(instance.statMultiplier!=null)
        {
            statMultiplier = new float[instance.statMultiplier.Length];
            for (int i = 0; i < instance.statMultiplier.Length; i++)
            {
                statMultiplier[i] = instance.statMultiplier[i];
            }
        }
    }

    public Attachment attachment => (Attachment)Data;

    public void Init()
    {
        statMultiplier = new float[attachment.StatusDatas.Length];

        for (int i = 0; i < attachment.StatusDatas.Length; i++)
        {
            if (attachment.StatusDatas[i].StatusType == AttachmentStatusType.extraDamage)
                statMultiplier[i] = Random.Range(0.8f,1.2f);
            else if (attachment.StatusDatas[i].StatusType == AttachmentStatusType.extraNoise)
                statMultiplier[i] = Random.Range(0f, 0.9f);
            else if (attachment.StatusDatas[i].StatusType == AttachmentStatusType.extraMagazine)
                statMultiplier[i] = Random.Range(0.8f, 1.2f);
            else if (attachment.StatusDatas[i].StatusType == AttachmentStatusType.extraReloadTime)
                statMultiplier[i] = Random.Range(0.8f, 1.2f);
            else if (attachment.StatusDatas[i].StatusType == AttachmentStatusType.extraAccuracy)
                statMultiplier[i] = Random.Range(0.8f, 1.2f);
            else if (attachment.StatusDatas[i].StatusType == AttachmentStatusType.extraRecoil)
                statMultiplier[i] = -Random.Range(0.8f, 1.2f);
            else if (attachment.StatusDatas[i].StatusType == AttachmentStatusType.extraReduce)
                statMultiplier[i] = Random.Range(0.8f, 1.2f);
            else if (attachment.StatusDatas[i].StatusType == AttachmentStatusType.extraAmmo)
                statMultiplier[i] = Random.Range(0.8f, 1.2f);
            else if (attachment.StatusDatas[i].StatusType == AttachmentStatusType.extraSpeed)
                statMultiplier[i] = -Random.Range(0.8f, 1.2f);
        }
    }

    public float FinalStat(float stat, float Multiplier)
    {
        int s = (int)(stat * Multiplier);
        return s;
    }

    public float[] FinalStats()
    {
        int count = attachment.StatusDatas.Length;
        float[] s = new float[count];

        for(int i=0;i<count;i++)
        {
            s[i] = FinalStat(attachment.StatusDatas[i].Value, statMultiplier[i]);
        }

        return s;
    }

    public string DescStr()
    {
        sb.Clear();

        for(int i=0;i<attachment.StatusDatas.Length;i++)
        {
            sb.AppendLine($"{attachment.StatusDatas[i].StatusType} : {FinalStat(attachment.StatusDatas[i].Value, statMultiplier[i])}");
        }

        return sb.ToString();
    }

}
