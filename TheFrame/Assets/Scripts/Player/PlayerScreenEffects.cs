using TMPro;
using Unity.Cinemachine;
using UnityEngine;

//WeaponEffect로 변경, 카메라 반동, 총구 화염, 발사 음 관리
public class PlayerScreenEffects : MonoBehaviour
{
    //탄창 텍스트
    public TextMeshProUGUI AmmoText;

    //재장전 텍스트
    public TextMeshProUGUI reloadText;

    //총구 화염
    public ParticleSystem muzzleFlash;
    
    //카메라
    public CinemachineCamera cinemachineCamera;

    [Header("Recoil")]
    public Transform pivot;                         //총기 반동용 타겟

    private Vector2 currentPos;                 //반동용 포지션
    private Vector2 targetPos;                  //반동용 포지션

    //에임 줌인, 줌아웃, 변경 스피드
    private float minFov = 30f;
    private float maxFov = 60f;
    private float currentFov = 60f;
    private float aimSpeed = 10f;

    //반동 세기
    private float recoil = 0f;
    private float halfRecoil = 0f;

    private void Update()
    {
        RecoilPivot();
        ChangeFov();
    }

    //발사 코드
    public void FireEffects()
    {
        muzzleFlash.Play();

        FireRecoil();
    }

    //발사 할 때 반동
    public void FireRecoil()
    {
        targetPos += new Vector2(Random.Range(-halfRecoil, halfRecoil), recoil);
    }

    //반동 제어
    public void RecoilPivot()
    {
        targetPos = Vector2.Lerp(targetPos, Vector2.zero, 5f * Time.deltaTime);
        currentPos = Vector2.Lerp(currentPos, targetPos, 10f * Time.deltaTime);

        // Pivot 위치 이동
        pivot.localPosition = new Vector3(-currentPos.x, currentPos.y, 0f);
    }

    //무기 변경 시마다 반동 세기 설정
    public void SetRecoil(float recoil)
    {
        this.recoil = recoil / 200f;
        halfRecoil = this.recoil / 2f;
    }

    //현재 탄약 텍스트
    public void UpdateAmmoText(float currentAmmo)
    {
        AmmoText.text = $"{currentAmmo}";
    }

    public void ShowReloadText()
    {
        reloadText.text = $"Reload...";
        //재장전 시작 시 텍스트
        reloadText.gameObject.SetActive(true);
    }

    public void HideReloadText()
    {
        //재장전 완료 시 텍스트
        reloadText.gameObject.SetActive(false);
    }

    //화면 확대, 축소
    void ChangeFov()
    {
        cinemachineCamera.Lens.FieldOfView =
            Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, currentFov, Time.deltaTime * aimSpeed);
    }

    //조준 했는지 체크
    public void CheckAim(bool isAiming)
    {
        currentFov = isAiming ? minFov : maxFov;
    }
}
