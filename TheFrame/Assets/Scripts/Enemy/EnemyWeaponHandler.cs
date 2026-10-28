using UnityEngine;

public class EnemyWeaponHandler : MonoBehaviour
{
    //최소 명중률
    float minAccuracy = 0f;
    //최대 명중률
    float maxAccuracy = 0f;     
    //현재 명중률
    float currentAccuracy = 0f;

    //재장전 실행 중인지 확인
    public bool IsReload { get; private set; }

    public WeaponController weaponController;

    private void Update()
    {
        UpdateAccuracy();

        if(weaponController.IsReloading())
        {
            weaponController.Reloading(Time.deltaTime);
        }
    }

    public void Init(WeaponInstance weapon)
    {
        minAccuracy = weapon.FinalRecoil() * ((100f - weapon.FinalAccuracy()) / 100f);
        maxAccuracy = weapon.FinalRecoil() * 2f;
        weaponController.SetWeapon(weapon);
    }

    //발사 실행
    public bool Fire(Vector3 targetPos)
    {
        //발사가 가능한지 체크
        if (weaponController.TryFire())
        {
            Shoot(targetPos);
            AccuracyClaculator();
            return true;
        }
        else
        {
            //발사를 못할 때 탄약이 없는지 체크
            if (weaponController.IsAmmoEmpty())
            {
                if (weaponController.TryReload())
                {
                    //재장전 상태 실행
                    //재장전 애니메이션 실행
                    IsReload = true;
                }
            }
        }

        return false;
    }

    // 여기부터 시작-------------------------------------------------------------------
    void Shoot(Vector3 targetPos)
    {
        //적의 명중률
        Vector2 circle = Random.insideUnitCircle * currentAccuracy;
        //타겟의 위치에 좌우위아래 랜덤한 원형 범위 추가
        Vector3 targetPoint =
            new Vector3((targetPos.x + circle.x), (targetPos.y + circle.y), targetPos.z);

        weaponController.Shoot(targetPoint);
    }

    //재장전이 끝났는지 체크
    public void CheckReloadIsFinish(float deltaTime)
    {
        if(weaponController.IsReloading())
        {
            if (weaponController.Reloading(deltaTime))
            {
                IsReload = false;
            }
        }
    }

    void UpdateAccuracy()
    {
        currentAccuracy =
            Mathf.Lerp(currentAccuracy, maxAccuracy, 2f * Time.deltaTime);
    }

    void AccuracyClaculator()
    {
        currentAccuracy -= 2f;
        currentAccuracy =
            Mathf.Clamp(currentAccuracy, minAccuracy, maxAccuracy);
    }

}
