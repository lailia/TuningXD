using UnityEngine;

public class CrossHairController : MonoBehaviour
{
    public RectTransform[] crossHairs;              //0 : up, 1 : down, 2 : ringht, 3 : left

    private float minSpread;            //조준점 최소 거리
    private float maxSpread;            //조준점 최대 거리
    private float currentSpread;        //조준점 현재 거리
    private float spreadSpeed;          //조준점 벌어질 속도
    private float spreadRecover;        //조준점 회복 속도
    private float move;                 //움직일 때 벌어질 거리
    private float add;                  //조준 중 감소할 거리

    //무기 교체 시 수치 세팅 메서드
    public void SetSpread(WeaponInstance weapon)
    {
        minSpread = weapon.FinalRecoil() * ((100f - weapon.FinalAccuracy()) / 100f);
        maxSpread = weapon.FinalRecoil() * 2f;
        spreadSpeed = minSpread / 2f;
        spreadRecover = 5f;
    }

    //실시간 조준점 변경 메서드
    public void UpdateSpread(bool isMove, bool isAim)
    {
        //움직일 때 
        move = isMove ? 20f : 0f;
        //조준할 때
        add = isAim ? -10f : 0f;

        //발사를 안할 때 조준점 줄어드는 코드
        currentSpread = 
            Mathf.Lerp(currentSpread, (minSpread + move + add), spreadRecover * Time.deltaTime);
        
        //currentSpread에 따라 조준점 변경하는 코드
        crossHairs[0].anchoredPosition = new Vector3(0f, currentSpread, 0f);
        crossHairs[1].anchoredPosition = new Vector3(0f, -currentSpread, 0f);
        crossHairs[2].anchoredPosition = new Vector3(currentSpread, 0f, 0f);
        crossHairs[3].anchoredPosition = new Vector3(-currentSpread, 0f, 0f);
    }

    //발사 시 조준점 벌어지는 메서드
    public float SpreadCalculater()
    {
        float current = currentSpread;

        currentSpread += spreadSpeed;
        currentSpread = Mathf.Clamp(currentSpread, (minSpread + move + add), (maxSpread + move + add));

        return current;
    }
}
