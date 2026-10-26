using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    //방어구
    public ArmorInstance armor;

    //애니메이터
    public Animator playerAni;                     

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
        MoveAnimation(playerController.PlayerMove(),
            playerController.currentSpeed,
            playerController.IsRunning);

        playerWeaponHandler.UpdateCrossHair(playerController.IsMoving, playerController.IsAimInput);

        if(playerController.IsAttackInput)
        {
            if(playerWeaponHandler.IsFire())
            {
                FireAni();
            }
        }

        if (playerController.ReloadInput())
        {
            playerWeaponHandler.TryReload();
        }
    }

    private void LateUpdate()
    {
        playerController.PlayerLook(lookDistance);
    }

    //장착 무기 변경
    public void SetWeapon(WeaponInstance weapon)
    {
        if (weapon!=null)
        {
            playerWeaponHandler.SetWeapon(weapon);
        }
        else
        {
            playerWeaponHandler.SetWeapon(weapon);
        }
    }

    //장착 방어구 변경
    public void SetArmor(ArmorInstance armor)
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

    void MoveAnimation(Vector2 dir, float speed, bool isRunning)
    {
        playerAni.SetFloat("X", dir.x);
        playerAni.SetFloat("Y", dir.y);
        playerAni.SetFloat("Speed", speed);
        playerAni.SetBool("Aiming", !isRunning);
    }

    void FireAni()
    {
        playerAni.SetTrigger("Shoot");
    }

    void ReloadAni(bool reload)
    {
        playerAni.SetBool("Reload", reload);
    }
}
