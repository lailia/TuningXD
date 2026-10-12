using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    //무기
    public WeaponInstance weapon;
    //방어구
    public ArmorInstance armor;

    //탄창 텍스트
    public TextMeshProUGUI magazineText;

    //기본 스탯
    public float reduce;
    public float health;
    public float maxAmmo;
    public float speed;

    public bool isEquipWeapon;

    public PlayerController playerController;
    public WeaponController weaponController;
    public CrossHairController crossHairController;
    public PlayerScreenEffects playerScreenEffects;

    void Update()
    {
        //조준점 업데이트 할 때 플레이어 이동과 조준 실행
        crossHairController.SpreadUpdate(playerController.PlayerMove(), playerController.AimCheck());
        //반동 제어
        playerScreenEffects.RecoilPivot();
    }

    void Fire()
    {
        if(playerController.isFire)
        {

        }
    }

    public void WeaponSet(WeaponInstance weapon)
    {
        if (weapon!=null)
        {
            this.weapon = weapon;
            weaponController.WeaponSet(this.weapon);

            isEquipWeapon = true;
        }
        else
        {
            this.weapon = null;
            weaponController.WeaponSet(this.weapon);

            isEquipWeapon = false;
        }
    }

    public void ArmorSet(ArmorInstance armor)
    {
        if(armor!=null)
        {
            this.armor = armor;
        }
        else
        {
            this.armor = null;
        }
    }

    public void UpdateMagazineText()
    {
        magazineText.text = $"{weapon.CurrentMagazine}";
    }

}
