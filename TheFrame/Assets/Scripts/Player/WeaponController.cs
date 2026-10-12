using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class WeaponController : MonoBehaviour
{
    //총구 각도랑 피격 각도의 차이를 제한 할 때 사용하는 변수
    [Header("Angle")]
    public float maxAngle;

    //총구 위치
    public Transform shootTransform;

    //총구 화염 이펙트
    public ParticleSystem muzzleFlash;

    //무기가 사용하는 총알
    public GameObject bulletPrefab;       
    
    //총알 자국
    public GameObject bulletHolePrefab;

    //풀매니저
    public ObjectPoolManager poolManager;

    //무기
    [SerializeField]
    private WeaponInstance weapon;
    public WeaponInstance Weapon => weapon;

    private float reload = 0f;

    //무기 발사 메서드
    //플레이어 전용 발사 실행-------------------------------------------------여기부터 시작
    public void Shoot(float lookDistance, ParticleSystem muzzleFlash)
    {
        muzzleFlash.Play();

        //레이를 쏠 시작점 구하기
        Vector2 center = new Vector2(Screen.width, Screen.height) * 0.5f;
        Vector2 spreadOffset = Random.insideUnitCircle;// * current 현재 조준점 비례 탄퍼짐
        Vector2 screenPos = center + spreadOffset;

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
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

        Vector3 v = Camera.main.transform.position + (Camera.main.transform.forward * lookDistance);

        Vector3 dir = (targetPoint - shootTransform.position).normalized;                        //방향
        float angle = Vector3.Angle(shootTransform.forward, dir);                   //각도
        Vector3 axis = Vector3.Cross(shootTransform.forward, dir);                  //회전축
        float clampedAngle = Mathf.Min(angle, maxAngle);

        Vector3 fireDir = Quaternion.AngleAxis(clampedAngle, axis) * shootTransform.forward;

        float dis = Vector3.Distance(v, shootTransform.position);                   //총구에서 바라본 거리

        ray = new Ray(shootTransform.position, fireDir);                             //총구부터 시작하는 레이

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

    //무기 재장전 메서드
    public bool ReloadCheck()
    {
        //수정 필요
        if (Weapon.CurrentMagazine > 0)
        {
            Weapon.Shoot();
            return true;
        }
        else
        {
            //reload = 0f;
            return false;
        }
    }

    public void Reloaded()
    {
        //수정 필요
        if(weapon.weapon!=null)
        {
            reload += Time.deltaTime;
            if (reload >= Weapon.FinalReloadTime())
            {
                Weapon.Reload();
                reload = 0f;
            }
        }
    }

    //무기 세팅
    public void WeaponSet(WeaponInstance weapon)
    {
        //수정 필요
        this.weapon = weapon;
    }

}
