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

    void Start()
    {
        
    }

    public void Init(WeaponInstance weapon)
    {
        minSpread = weapon.FinalRecoil() * ((100f - weapon.FinalAccuracy()) / 100f);
        maxSpread = weapon.FinalRecoil() * 2f;
        spreadSpeed = minSpread / 2f;
        spreadRecover = 5f;
    }

    void Update()
    {
        SpreadRecover();
        CrossHairUpdate();
    }

    public void SetCurrentSpread(float _currentSpread)
    {
        currentSpread = _currentSpread;
    }

    private void CrossHairUpdate()
    {
        crossHairs[0].anchoredPosition = new Vector3(0f, currentSpread, 0f);
        crossHairs[1].anchoredPosition = new Vector3(0f, -currentSpread, 0f);
        crossHairs[2].anchoredPosition = new Vector3(currentSpread, 0f, 0f);
        crossHairs[3].anchoredPosition = new Vector3(-currentSpread, 0f, 0f);
    }

    //이동 할 때 조준점 벌어지는 거리
    public void SetMove(bool isMove)
    {
        //플레이어 전용
        move = isMove ? 20f : 0f;
    }

    //줌 했을 때 줄어들 거리
    public void SetAdd(bool isAim)
    {
        //플레이어 전용
        add = isAim ? -10f : 0f;
    }

    //조준점 제어
    public void SpreadRecover()
    {
        //플레이어 전용
        currentSpread = Mathf.Lerp(currentSpread, (minSpread + move + add), spreadRecover * Time.deltaTime);
    }

    //조준점 계산
    public void SpreadCalculater()
    {
        //플레이어 전용
        currentSpread += spreadSpeed;
        currentSpread = Mathf.Clamp(currentSpread, (minSpread + move + add), (maxSpread + move + add));
    }
}
