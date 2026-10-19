using UnityEngine;
using UnityEngine.InputSystem;

//플레이어의 무기 관리 클래스
public class PlayerWeaponHandler : MonoBehaviour
{
    //발사 가능한지 체크
    bool isFire = false;
    //조준중인지 체크
    bool isAiming = false;

    //무기
    public WeaponInstance weapon;

    //무기 컨트롤러
    public WeaponController weaponController;

    //플레이어의 무기 이펙트
    public PlayerScreenEffects playerScreenEffects;

    //크로스 헤어 컨트롤러
    public CrossHairController crossHairController;

    //총구 위치
    public Transform ShootTransform;

    private void Update()
    {
        //반동 제어
        playerScreenEffects.RecoilPivot();

        weapon.Tick(Time.deltaTime);
    }

    //발사 실행
    public void Fire(float lookDistance)
    {
        if (isFire)
        {
            if (weapon.TryFire())
            {
                playerScreenEffects.FireEffects(weapon);
                Shoot(crossHairController.SpreadCalculater(), lookDistance);
            }
        }
    }

    void Shoot(float current, float lookDistance)
    {
        //레이를 쏠 시작점 구하기
        Vector2 center = new Vector2(Screen.width, Screen.height) * 0.5f;
        Vector2 spreadOffset = Random.insideUnitCircle * current;
        Vector2 screenPos = center + spreadOffset;

        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        Debug.DrawRay(ray.origin, ray.direction*100f, Color.red);

        weaponController.Shoot(ray, ShootTransform, lookDistance);
    }
    
    //재장전
    public void Reload()
    {
        weaponController.Reload(weapon);
    }

    public void WeaponSet(WeaponInstance weapon)
    {
        crossHairController.SpreadSet(weapon);
        this.weapon = weapon;
    }

    public void CrossHairUpdate(bool isMove)
    {
        crossHairController.SpreadUpdate(isMove, isAiming);
    }

    //오른쪽 클릭 했을 때 (조준)
    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }

        playerScreenEffects.AimCheck(isAiming);
    }

    //왼쪽 클릭 했을 때 (공격)
    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isFire = true;
        }
        else
        {
            isFire = false;
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Reload();
        }
    }
}
