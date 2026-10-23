using UnityEngine;
using UnityEngine.InputSystem;

//플레이어의 무기 관리 클래스
public class PlayerWeaponHandler : MonoBehaviour
{
    //발사 가능한지 체크
    bool isAttackInput = false;
    //조준중인지 체크
    bool isAimInput = false;

    //무기 컨트롤러
    public WeaponController weaponController;

    //플레이어의 무기 이펙트
    public PlayerScreenEffects playerScreenEffects;

    //크로스 헤어 컨트롤러
    public CrossHairController crossHairController;

    private void Update()
    {
        // 발사 버튼을 누르고 있으면
        // 틱 계산해서 발사가 된다.

        if(isAttackInput)
        {
            Fire();
        }

        // 재장전 중일 때만, 재장전이 끝났는지를 체크할 거다.
        if (weaponController.IsReloading())
        {
            CheckReloadIsFinish();
        }
    }

    //발사 실행
    public void Fire()
    {
        if (weaponController.TryFire())
        {
            //플레이어 전용 화면 반동
            playerScreenEffects.FireRecoil();

            //발사 기능
            Shoot(crossHairController.SpreadCalculater(), weaponController.weapon.FinalRange());

            //현재 남은 탄약 텍스트
            playerScreenEffects.UpdateAmmoText(weaponController.weapon.CurrentAmmo);
        }
        else
        {
            if (weaponController.IsAmmoEmpty())
            {
                TryReload();
            }
        }
    }

    void Shoot(float current, float shootRange)
    {
        RaycastHit hit;

        Vector3 targetPoint;

        //레이를 쏠 시작점 구하기
        Vector2 center = new Vector2(Screen.width, Screen.height) * 0.5f;
        Vector2 spreadOffset = Random.insideUnitCircle * current;
        Vector2 screenPos = center + spreadOffset;

        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out hit, shootRange))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * shootRange;
        }

        weaponController.Shoot(targetPoint);
    }

    //재장전 시작
    public void TryReload()
    {
        if (weaponController.TryReload())
            playerScreenEffects.ShowReloadText();
    }
    
    //재장전 종료
    void CheckReloadIsFinish() // CheckReloadIsFinish
    {
        if (weaponController.Reloading(Time.deltaTime))
        {
            playerScreenEffects.HideReloadText();      
            playerScreenEffects.UpdateAmmoText(weaponController.weapon.CurrentAmmo);
        }
    }

    public void SetWeapon(WeaponInstance weapon)
    {
        crossHairController.SetSpread(weapon);
        weaponController.SetWeapon(weapon);
        playerScreenEffects.SetRecoil(weapon.FinalRecoil());
        playerScreenEffects.UpdateAmmoText(weapon.CurrentAmmo);
    }

    public void UpdateCrossHair(bool isMove)
    {
        crossHairController.UpdateSpread(isMove, isAimInput);
    }

    //오른쪽 클릭 했을 때 (조준)
    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isAimInput = true;
        }
        else
        {
            isAimInput = false;
        }

        playerScreenEffects.CheckAim(isAimInput);
    }

    //왼쪽 클릭 했을 때 (공격)
    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isAttackInput = true;
        }
        else
        {
            isAttackInput = false;
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            TryReload();
        }
    }
}
