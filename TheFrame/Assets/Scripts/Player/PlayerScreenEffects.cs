using TMPro;
using UnityEngine;

public class PlayerScreenEffects : MonoBehaviour
{
    //재장전 텍스트
    public TextMeshProUGUI reloadText;

    [Header("Recoil")]
    public Transform pivot;                         //총기 반동용 타겟

    private Vector2 currentPos;                 //반동용 포지션
    private Vector2 targetPos;                  //반동용 포지션

    //발사 할 때 실행될 코드
    public void FireRecoil(WeaponInstance weapon)
    {
        //반동 추가
        float recoil = weapon.FinalRecoil() / 200f;
        float halfRecoil = recoil / 2f;
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

    //재장전 텍스트
    void Reload()
    {
        //재장전 완료 시 텍스트
        reloadText.gameObject.SetActive(false);



        reloadText.text = $"Reload...";
        //재장전 시작 시 텍스트
        reloadText.gameObject.SetActive(true);
    }
}
