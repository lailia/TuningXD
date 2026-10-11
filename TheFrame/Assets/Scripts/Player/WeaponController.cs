using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class WeaponController : MonoBehaviour
{
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
