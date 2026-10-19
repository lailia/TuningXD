using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

//발사, 재장전, 발사 쿨타임 관리
public class WeaponController : MonoBehaviour
{
    //총구 각도랑 피격 각도의 차이를 제한 할 때 사용하는 변수
    [Header("Angle")]
    public float maxAngle;

    //무기가 사용하는 총알
    public GameObject bulletPrefab;       
    
    //총알 자국
    public GameObject bulletHolePrefab;

    //풀매니저
    public ObjectPoolManager poolManager;

    //발사
    public void Shoot(Ray ray, Transform shootTransform, float lookDistance)
    {
        //이 아래로 공용 코드
        RaycastHit hit;

        Vector3 targetPoint;
        Vector3 hitNormal = Vector3.zero;

        if (Physics.Raycast(ray, out hit, lookDistance))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * lookDistance;
        }

        //명중률 계산 :  hitPos = (hit.point + random * (반동 * ((100 - 명중률)/100f)))에 맞추기
        //조준점 계산 :  distance(14 ~ 28) = currentRecoil(20 ~ 40) * ((100-명중률(30))/100f)

        //목표 위치-----------------------
        //Vector3 v = Camera.main.transform.position + (Camera.main.transform.forward * lookDistance);

        Vector3 dir = (targetPoint - shootTransform.position).normalized;                        //방향
        float angle = Vector3.Angle(shootTransform.forward, dir);                   //각도
        Vector3 axis = Vector3.Cross(shootTransform.forward, dir);                  //회전축
        float clampedAngle = Mathf.Min(angle, maxAngle);

        Vector3 fireDir = Quaternion.AngleAxis(clampedAngle, axis) * shootTransform.forward;

        //총구에서 바라본 거리
        float dis = Vector3.Distance(shootTransform.position, targetPoint);

        //총구부터 시작하는 레이
        ray = new Ray(shootTransform.position, fireDir);                             

        //카메라 레이 먼저 쏘고 총구에서 레이 쏴서 각도 내에 있을 시 정상 작동, 최대 각도를 넘어가면 최대각도만큼만
        //조준점 클래스 만들고 총 컨트롤러와 플레이어 컨트롤러에서 관리
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
            hitNormal = hit.normal;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * dis;
        }

        BulletFire(targetPoint, hitNormal, shootTransform);
        //발사 후 조준점 벌어지는 코드 추가 필요
    }

    //총알 및 총알 자국 생성
    private void BulletFire(Vector3 hitPos, Vector3 normal,Transform shootTransform)
    {
        //공통
        if (!normal.Equals(Vector3.zero))
        {
            //총알 자국
            GameObject bhObj = poolManager.GetObject(bulletHolePrefab);
            BulletHole bh = bhObj.GetComponent<BulletHole>();
            bh.Init(bulletHolePrefab, poolManager);
            bh.transform.position = hitPos;
            bh.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
            bh.EffectPlay();
        }

        SoundManager.Instance.PlaySFX(SoundManager.Instance.fireClip);

        //총알
        GameObject bObj = poolManager.GetObject(bulletPrefab, out ObjectPool<GameObject> pool);
        Bullet b = bObj.GetComponent<Bullet>();
        b.Init(bulletPrefab, poolManager);
        b.transform.position = shootTransform.position;
        b.transform.up = (hitPos - b.transform.position).normalized;
    }

    //재장전
    public void Reload(WeaponInstance weapon)
    {
        weapon.StartReload();
    }

}
