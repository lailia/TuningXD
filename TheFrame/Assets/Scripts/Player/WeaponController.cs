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

    //무기
    public WeaponInstance weapon;

    //총구 위치
    public Transform ShootTransform;

    //무기가 사용하는 총알
    public GameObject bulletPrefab;       
    
    //총알 자국
    public GameObject bulletHolePrefab;

    //풀매니저
    public ObjectPoolManager poolManager;

    private void Update()
    {
        //총의 발사 쿨타임
        weapon.Tick(Time.deltaTime);
    }

    public bool Reloading(float deltaTime)
    {
        return weapon.ReloadTick(deltaTime);
    }

    public void SetWeapon(WeaponInstance weapon)
    {
        this.weapon = weapon;
    }

    //발사
    public void Shoot(Vector3 targetPoint)
    {
        //목표 위치-----------------------
        //Vector3 v = Camera.main.transform.position + (Camera.main.transform.forward * lookDistance);

        RaycastHit hit;

        Vector3 hitNormal = Vector3.zero;

        Vector3 dir = (targetPoint - ShootTransform.position).normalized;                        //방향
        float angle = Vector3.Angle(ShootTransform.forward, dir);                   //각도
        Vector3 axis = Vector3.Cross(ShootTransform.forward, dir);                  //회전축
        float clampedAngle = Mathf.Min(angle, maxAngle);

        Vector3 fireDir = Quaternion.AngleAxis(clampedAngle, axis) * ShootTransform.forward;

        //총구에서 바라본 거리
        float dis = Vector3.Distance(ShootTransform.position, targetPoint);

        //총구부터 시작하는 레이
        Ray ray = new Ray(ShootTransform.position, fireDir);                             

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
            hitNormal = hit.normal;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * dis;
        }

        BulletFire(targetPoint, hitNormal);
    }

    //총알 및 총알 자국 생성
    private void BulletFire(Vector3 hitPos, Vector3 normal)
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
        b.transform.position = ShootTransform.position;
        b.transform.up = (hitPos - b.transform.position).normalized;
    }

    //재장전
    public void StartReload()
    {
        weapon.StartReload();
    }

}
