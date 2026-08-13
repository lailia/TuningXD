using UnityEngine;

public class Player : MonoBehaviour
{
    //무기
    public WeaponInstance Weapon { get; private set; }
    //방어구
    public ArmorInstance Armor { get; private set; }

    //기본 스탯
    public float Reduce { get; private set; }
    public float Health { get; private set; }
    public float MaxAmmo { get; private set; }
    public float Speed { get; private set; }

    public bool IsEquipWeapon { get; private set; }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void WeaponSet(WeaponInstance weapon)
    {
        if (weapon!=null)
        {
            this.Weapon = weapon;

            IsEquipWeapon = true;
        }
        else
        {
            this.Weapon = null;

            IsEquipWeapon = false;
        }
    }

    public void ArmorSet(ArmorInstance armor)
    {
        if(armor!=null)
        {
            this.Armor = armor;
        }
        else
        {
            this.Armor = null;
        }
    }

}
