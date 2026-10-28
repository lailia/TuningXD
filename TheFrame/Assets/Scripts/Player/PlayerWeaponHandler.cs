using UnityEngine;
using UnityEngine.InputSystem;

//플레이어의 무기 관리 클래스
public class PlayerWeaponHandler : MonoBehaviour
{
    //무기 컨트롤러
    public WeaponController weaponController;

    //플레이어의 무기 이펙트
    public PlayerScreenEffects playerScreenEffects;

    //크로스 헤어 컨트롤러
    public CrossHairController crossHairController;

    //발사 실행
    public bool IsFire()
    {
        if (weaponController.TryFire())
        {
            //플레이어 전용 화면 반동
            playerScreenEffects.FireRecoil();

            //발사 기능
            Shoot(crossHairController.SpreadCalculator(), weaponController.weapon.FinalRange());

            //현재 남은 탄약 텍스트
            playerScreenEffects.UpdateAmmoText(weaponController.weapon.CurrentAmmo);

            return true;
        }

        return false;
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

        if (Physics.Raycast(ray, out hit, shootRange, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
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
    public bool TryReload()
    {
        if (weaponController.TryReload())
        {
            playerScreenEffects.ShowReloadText();
            return true;
        }
        return false;
    }
    
    //재장전 종료
    public bool CheckReloadIsFinish()
    {
        if (weaponController.Reloading(Time.deltaTime))
        {
            playerScreenEffects.HideReloadText();      
            playerScreenEffects.UpdateAmmoText(weaponController.weapon.CurrentAmmo);
            return true;
        }
        return false;
    }

    public void SetWeapon(WeaponInstance weapon)
    {
        crossHairController.SetSpread(weapon);
        weaponController.SetWeapon(weapon);
        playerScreenEffects.SetRecoil(weapon.FinalRecoil());
        playerScreenEffects.UpdateAmmoText(weapon.CurrentAmmo);
    }

    public void UpdateCrossHair(bool isMove, bool isAimInput)
    {
        crossHairController.UpdateSpread(isMove, isAimInput);
        playerScreenEffects.CheckAim(isAimInput);
    }
}
