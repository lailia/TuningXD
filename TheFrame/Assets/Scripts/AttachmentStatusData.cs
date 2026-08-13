
public enum AttachmentStatusType
{
    extraDamage,
    extraRecoil,
    extraAccuracy,
    extraMagazine,
    extraReloadTime,
    extraNoise,
    extraReduce,
    extraAmmo,
    extraSpeed
}

[System.Serializable]
public class AttachmentStatusData
{
    public AttachmentStatusType StatusType;
    public float Value;

    public void Set(AttachmentStatusType _statusType, float _value)
    {
        this.StatusType = _statusType;
        this.Value = _value;
    }

    //public AttachmentStatusData Clone()
    //{
    //    return new AttachmentStatusData
    //    {
    //        StatusType = statusType,
    //        value = value
    //    };
    //}
}
