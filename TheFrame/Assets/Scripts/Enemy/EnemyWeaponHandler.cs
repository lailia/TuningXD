using UnityEngine;

public class EnemyWeaponHandler : MonoBehaviour
{
    public WeaponController weaponController;

    private void Update()
    {
        
    }

    //발사 실행
    public void Fire()
    {
        //발사가 가능한지 체크
        if(weaponController.TryFire())
        {

        }
        else
        {
            //발사를 못할 때 탄약이 없는지 체크
            if(weaponController.IsAmmoEmpty())
            {

            }
        }
    }

    // 여기부터 시작-------------------------------------------------------------------
    void Shoot(Vector3 targetPos)
    {
        //타겟 위치 + 랜덤 원형 범위(x,y)

        //타겟 위치
        //targetPos;      
        //vector2 Circle = Random.insideUnitCircle * 원의 범위;

        //targetPos + Circle;

    }

}
