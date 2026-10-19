using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    //방어구
    public ArmorInstance armor;

    //기본 스탯
    public float reduce;
    public float health;
    public float maxAmmo;
    public float speed;

    //플레이어가 바라볼 카메라 거리
    public float lookDistance;              

    public PlayerController playerController;
    public PlayerWeaponHandler playerWeaponHandler;

    private void Update()
    {
        playerWeaponHandler.Fire(lookDistance);
        playerWeaponHandler.CrossHairUpdate(playerController.PlayerMove());
    }

    private void LateUpdate()
    {
        playerController.PlayerLook(lookDistance);
    }

    //장착 무기 변경
    public void WeaponSet(WeaponInstance weapon)
    {
        if (weapon!=null)
        {
            playerWeaponHandler.WeaponSet(weapon);
        }
        else
        {
            playerWeaponHandler.WeaponSet(weapon);
        }
    }

    //장착 방어구 변경
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

}
