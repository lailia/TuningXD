using System.Text;
using UnityEngine;

[System.Serializable]
public class WeaponInstance : ItemInstance
{
    public float DamageMultiplier { get; private set; }
    public float RpmMultiplier { get; private set; }
    public float RecoilMultiplier { get; private set; }
    public float AccuracyMultiplier { get; private set; }
    public float MagazineMultiplier { get; private set; }
    public float ReloadMultiplier { get; private set; }
    public float NoiseMultiplier { get; private set; }

    private float PlusDamage;
    private float PlusRpm;
    private float PlusRecoil;
    private float PlusAccuracy;
    private float PlusMagazine;
    private float PlusReload;
    private float PlusNoise;

    //현재 남은 총알
    public float CurrentMagazine { get; private set; }

    //현재 발사 쿨타임
    public float FireCooldown { get; private set; }

    //재장전 중인지 체크
    public bool IsReloading { get; private set; }

    //현재 재장전 쿨타임
    public float CurrentReloadTime { get; private set; }

    //0 : muzzle, 1 : magazine, 2 : scope, 3 : stock
    private AttachmentInstance[] parts = new AttachmentInstance[4];  //파츠
    public AttachmentInstance[] Parts => parts;
    public StringBuilder sb = new StringBuilder();

    public WeaponInstance() : base() { }

    public WeaponInstance(Weapon data) : base(data) { }

    public WeaponInstance(WeaponInstance weaponIns) : base(weaponIns)
    {
        DamageMultiplier = weaponIns.DamageMultiplier;
        RpmMultiplier = weaponIns.RpmMultiplier;
        RecoilMultiplier = weaponIns.RecoilMultiplier;
        AccuracyMultiplier = weaponIns.AccuracyMultiplier;
        MagazineMultiplier = weaponIns.MagazineMultiplier;
        ReloadMultiplier = weaponIns.ReloadMultiplier;
        NoiseMultiplier = weaponIns.NoiseMultiplier;

        for (int i = 0; i < weaponIns.parts.Length; i++)
        {
            parts[i] = new AttachmentInstance(weaponIns.parts[i]);
        }

        Reload();
    }

    public Weapon weapon => (Weapon)Data;

    public void Init()
    {
        //스탯 랜덤 배율 설정
        DamageMultiplier = Random.Range(0.8f, 1.2f);
        RpmMultiplier = Random.Range(0.8f, 1.2f);
        RecoilMultiplier = Random.Range(0.8f, 1.2f);
        AccuracyMultiplier = Random.Range(0.8f, 1.2f);
        MagazineMultiplier = Random.Range(0.8f, 1.2f);
        ReloadMultiplier = Random.Range(0.8f, 1.2f);
        NoiseMultiplier = Random.Range(0.8f, 1.2f);

        //장착된 파츠 설정
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = new AttachmentInstance();
        }

        Reload();
    }

    public bool CheckParts(int index)
    {
        return parts[index].attachment != null;
    }

    public bool CheckParts(int index, out AttachmentInstance instance)
    {
        instance = parts[index];
        return instance.attachment != null;
    }

    public AttachmentInstance InstallParts(AttachmentInstance attachment)
    {
        AttachmentInstance instance = null;
        switch (attachment.attachment.partsType)
        {
            case PartsType.muzzle:
                if (parts[0].attachment != null)
                    instance = new AttachmentInstance(parts[0]);
                parts[0] = attachment;
                break;
            case PartsType.magazine:
                if (parts[1].attachment != null)
                    instance = new AttachmentInstance(parts[1]);
                parts[1] = attachment;
                break;
            case PartsType.scope:
                if (parts[2].attachment != null)
                    instance = new AttachmentInstance(parts[2]);
                parts[2] = attachment;
                break;
            case PartsType.stock:
                if (parts[3].attachment != null)
                    instance = new AttachmentInstance(parts[3]);
                parts[3] = attachment;
                break;
        }
        PlusStatSet();
        Reload();
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
        Reload();
        return attachment;
    }

    public void Copy(WeaponInstance weapon)
    {
        DamageMultiplier = weapon.DamageMultiplier;
        RpmMultiplier = weapon.RpmMultiplier;
        RecoilMultiplier = weapon.RecoilMultiplier;
        AccuracyMultiplier = weapon.AccuracyMultiplier;
        MagazineMultiplier = weapon.MagazineMultiplier;
        ReloadMultiplier = weapon.ReloadMultiplier;
        NoiseMultiplier = weapon.NoiseMultiplier;

        PlusDamage = weapon.PlusDamage;
        PlusRpm = weapon.PlusRpm;
        PlusRecoil = weapon.PlusRecoil;
        PlusAccuracy = weapon.PlusAccuracy;
        PlusMagazine = weapon.PlusMagazine;
        PlusReload = weapon.PlusReload;
        PlusNoise = weapon.PlusNoise;

        parts = weapon.parts;
    }

    //총알 발사 쿨타임 관리
    public void Tick(float deltaTime)
    {
        if (FireCooldown > 0f)
        {
            FireCooldown -= deltaTime;

            if (FireCooldown < 0f)
            {
                FireCooldown = 0f;
            }
        }

        if(IsReloading)
        {
            if(CurrentReloadTime>0f)
            {
                CurrentReloadTime -= deltaTime;

                if(CurrentReloadTime<0f)
                {
                    FinishReload();
                }
            }
        }
    }

    //현재 무기가 발사가 가능한 상태인지 확인
    public bool CanFire()
    {
        if (CurrentMagazine <= 0f)
            return false;
        if (FireCooldown > 0f)
            return false;
        if (IsReloading)
            return false;

        return true;
    }

    //발사 시도
    public bool TryFire()
    {
        if (!CanFire())
            return false;

        CurrentMagazine--;

        FireCooldown = 1f / (FinalRpm() / 60f);

        return true;
    }

    //재장전이 가능한 상태인지 체크
    public bool CanReload()
    {
        if (IsReloading)
            return false;
        if (CurrentMagazine >= FinalMagazine())
            return false;

        return true;
    }

    //재장전 시작
    public void StartReload()
    {
        if (!CanReload())
            return;

        CurrentReloadTime = FinalReloadTime();
        IsReloading = true;
    }

    //재장전 종료
    public void FinishReload()
    {
        if (!IsReloading)
            return;

        CurrentMagazine = FinalMagazine();
        IsReloading = false;
    }

    public void Reload()
    {
        CurrentMagazine = FinalMagazine();
    }

    public bool Shoot()
    {
        if (CurrentMagazine <= 0)
            return false;

        CurrentMagazine--;
        return true;
    }

    public void PlusStatSet()
    {
        PlusDamage = 0f;
        PlusRecoil = 0f;
        PlusAccuracy = 0f;
        PlusMagazine = 0f;
        PlusReload = 0f;
        PlusReload = 0f;
        PlusNoise = 0f;

        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].attachment != null)
            {
                for (int j = 0; j < parts[i].attachment.StatusDatas.Length; j++)
                {
                    AttachmentStatusData statusData = parts[i].attachment.StatusDatas[j];
                    switch (statusData.StatusType)
                    {
                        case AttachmentStatusType.extraDamage:
                            PlusDamage += statusData.Value;
                            break;
                        case AttachmentStatusType.extraRecoil:
                            PlusRecoil += statusData.Value;
                            break;
                        case AttachmentStatusType.extraAccuracy:
                            PlusAccuracy += statusData.Value;
                            break;
                        case AttachmentStatusType.extraMagazine:
                            PlusMagazine += statusData.Value;
                            break;
                        case AttachmentStatusType.extraReloadTime:
                            PlusReload += statusData.Value;
                            break;
                        case AttachmentStatusType.extraNoise:
                            PlusNoise += statusData.Value;
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

    public float FinalDamage()
    {
        return FinalStat(weapon.Damage, DamageMultiplier, PlusDamage);
    }

    public float FinalRpm()
    {
        return FinalStat(weapon.Rpm, RpmMultiplier, PlusRpm);
    }

    public float FinalRecoil()
    {
        return FinalStat(weapon.Recoil, RecoilMultiplier, PlusRecoil);
    }

    public float FinalAccuracy()
    {
        return FinalStat(weapon.Accuracy, AccuracyMultiplier, PlusAccuracy);
    }

    public float FinalMagazine()
    {
        return FinalStat(weapon.Magazine, MagazineMultiplier, PlusMagazine);
    }

    public float FinalPellet()
    {
        return FinalStat(weapon.Pellet, 1f, 0f);
    }

    public float FinalRange()
    {
        return FinalStat(weapon.Range, 1f, 0f);
    }

    public float FinalReloadTime()
    {
        return FinalStat(weapon.ReloadTime, ReloadMultiplier, PlusReload);
    }

    public float FinalNoise()
    {
        return FinalStat(weapon.Noise, NoiseMultiplier, PlusNoise);
    }

    public string DescStr()
    {
        sb.Clear();

        sb.AppendLine($"Damage : {FinalDamage()}");                //damage
        sb.AppendLine($"Rpm : {FinalRpm()}");                                      //rpm
        sb.AppendLine($"Recoil : {FinalRecoil()}");                //recoil
        sb.AppendLine($"Accuracy : {FinalAccuracy()}");            //accuracy
        sb.AppendLine($"Magazine : {FinalMagazine()}");            //magazine
        sb.AppendLine($"ReloadTime : {FinalReloadTime()}");        //reloadtime
        return sb.ToString();
    }

    public override AttachmentInstance[] GetPartsArray()
    {
        return parts;
    }
}
