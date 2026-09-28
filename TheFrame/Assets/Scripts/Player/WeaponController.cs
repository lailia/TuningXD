using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class WeaponController : MonoBehaviour
{
    public ParticleSystem muzzleFlash;              //총구 화염 이펙트
    public Transform shootTransform;                //총구 위치
    public TextMeshProUGUI magazineText;            //탄창 텍스트
    public TextMeshProUGUI reloadText;              //재장전 텍스트
    public GameObject bulletPrefab;                 //무기가 사용하는 총알
    public GameObject bulletHolePrefab;
    public ObjectPoolManager poolManager;
    public AudioSource audioSource;

    //무기
    [SerializeField]
    private WeaponInstance weapon;
    public WeaponInstance Weapon => weapon;
    private float reload = 0f;

    [Header("Recoil")]
    private float min;           //최소 반동
    private float max;           //최대값
    private float current;       //현재 거리
    private float speed;         //벌어질 속도
    private float recover;       //회복 속도
    private float move;          //움직일 때 벌어질 거리
    private float add;           //조준 중 감소할 거리

    [Header("Angle")]
    public float maxAngle;

    [Header("CrossHair")]
    public CrossHairController crossHairController;

    void Start()
    {
        
    }

    void Update()
    {
        SpreadRecover();
        crossHairController.SetCurrentSpread(current);
    }

    public void Init()
    {
        min = Weapon.FinalRecoil() * ((100f - Weapon.FinalAccuracy()) / 100f);
        max = Weapon.FinalRecoil() * 2f;
        speed = min / 2f;
        recover = 5f;

        magazineText.text = $"{Weapon.CurrentMagazine}";
    }

    public void Shoot(float lookDistance)
    {
        muzzleFlash.Play();

        //레이를 쏠 시작점 구하기
        Vector2 center = new Vector2(Screen.width, Screen.height) * 0.5f;
        Vector2 spreadOffset = Random.insideUnitCircle * current;
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

        //SpawnTrail(shootTransform.position, hitPos);
        BulletFire(targetPoint, hitNormal);
        SpreadCalculater();
    }

    private void BulletFire(Vector3 hitPos, Vector3 normal)
    {
        if (!normal.Equals(Vector3.zero))
        {
            GameObject bhObj = poolManager.GetObject(bulletHolePrefab);
            BulletHole bh = bhObj.GetComponent<BulletHole>();
            bh.Init(bulletHolePrefab, poolManager);
            bh.transform.position = hitPos;
            bh.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
            bh.EffectPlay();
        }

        SoundManager.Instance.PlaySFX(SoundManager.Instance.fireClip);

        // GameObject bObj = poolManager.GetObject(bulletPrefab);
        GameObject bObj = poolManager.GetObject(bulletPrefab, out ObjectPool<GameObject> pool);
        Bullet b = bObj.GetComponent<Bullet>();
        b.Init(bulletPrefab, poolManager);
        b.transform.position = shootTransform.position;
        b.transform.up = (hitPos - b.transform.position).normalized;
    }

    public float Rpm()
    {
        return Weapon.FinalRpm();
    }

    public void SpreadRecover()
    {
        current = Mathf.Lerp(current, (min + move + add), recover * Time.deltaTime);
    }

    public Vector2 FireRecoil()
    {
        float recoil = Weapon.FinalRecoil() / 200f;
        float halfRecoil = recoil / 2f;
        return new Vector2(Random.Range(-halfRecoil, halfRecoil), recoil);
    }

    public void SpreadCalculater()
    {
        current += speed;
        current = Mathf.Clamp(current, (min + move + add), (max + move + add));
    }

    public void SetMove(bool isMove)
    {
        move = isMove ? 20f : 0f;
    }

    public void SetAdd(bool isAim)
    {
        add = isAim ? -10f : 0f;
    }

    public bool ReloadCheck()
    {
        if (Weapon.CurrentMagazine > 0)
        {
            Weapon.Shoot();
            return true;
        }
        else
        {
            reloadText.text = $"Reload...";
            reloadText.gameObject.SetActive(true);
            //reload = 0f;
            return false;
        }
    }

    public void Reloaded()
    {
        if(weapon.weapon!=null)
        {
            reload += Time.deltaTime;
            if (reload >= Weapon.FinalReloadTime())
            {
                Weapon.Reload();
                reload = 0f;
                reloadText.gameObject.SetActive(false);
            }
        }
    }

    //무기 세팅
    public void WeaponSet(WeaponInstance weapon)
    {
        this.weapon = weapon;
    }
}
