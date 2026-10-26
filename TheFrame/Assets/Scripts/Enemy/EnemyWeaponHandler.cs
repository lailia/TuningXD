using UnityEngine;

public class EnemyWeaponHandler : MonoBehaviour
{
    float minAccuracy = 0f;     //최소 명중률
    float maxAccuracy = 0f;     //최대 명중률
    float currentAccuracy = 0f;

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
    public void Fire(Vector3 targetPos)
    {
        //발사가 가능한지 체크
        if (weaponController.TryFire())
        {
            Shoot(targetPos);
            AccuracyClaculator();
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
                }
            }
        }
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

    public void CheckReloadIsFinish(float deltaTime)
    {
        if(weaponController.IsReloading())
        {
            if (weaponController.Reloading(deltaTime))
            {
                //재장전 종료 실행
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
