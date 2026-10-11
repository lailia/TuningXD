using TMPro;
using UnityEngine;

public class PlayerFireSystem : MonoBehaviour
{
    //총구 각도랑 피격 각도의 차이를 제한 할 때 사용하는 변수
    [Header("Angle")]
    public float maxAngle;

    //총구 위치
    public Transform shootTransform;

    //재장전 텍스트
    public TextMeshProUGUI reloadText;              

    //공토 무기 컨트롤러
    public WeaponController weaponController;

    [Header("Recoil")]
    public Transform pivot;                         //총기 반동용 타겟

    private Vector2 currentPos;                 //반동용 포지션
    private Vector2 targetPos;                  //반동용 포지션

    //발사 실행
    public void Shoot(float lookDistance,ParticleSystem muzzleFlash)
    {
        //플레이어 전용
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

        BulletFire(targetPoint, hitNormal,shootTransform);
        //발사 후 조준점 벌어지는 코드 추가 필요
    }

    //재장전
    void Reload()
    {        
        //재장전 완료 시 텍스트
        reloadText.gameObject.SetActive(false);



        reloadText.text = $"Reload...";
        //재장전 시작 시 텍스트
        reloadText.gameObject.SetActive(true);
    }

    public float Rpm()
    {
        //수정 필요
        return Weapon.FinalRpm();
    }

    //반동
    public Vector2 FireRecoil()
    {
        //플레이어 전용
        float recoil = Weapon.FinalRecoil() / 200f;
        float halfRecoil = recoil / 2f;
        return new Vector2(Random.Range(-halfRecoil, halfRecoil), recoil);
    }

    //반동 제어
    private void RecoilPivot()
    {
        // 원래 위치로 복귀
        targetPos = Vector2.Lerp(targetPos, Vector2.zero, 5f * Time.deltaTime);
        currentPos = Vector2.Lerp(currentPos, targetPos, 10f * Time.deltaTime);

        // Pivot 위치 이동
        pivot.localPosition = new Vector3(-currentPos.x, currentPos.y, 0f);
    }

    //총기 반동
    private void FireRecoil()
    {
        targetPos += weaponController.FireRecoil();
    }

}
