using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour,
    IPointerDownHandler,                    //누를때 이벤트
    IDragHandler,                           //드래그할때 이벤트
    IPointerUpHandler                       //땔때 이벤트
{
    public RectTransform background;        //조이스틱 배경
    public RectTransform handle;            //조이스틱 핸들

    public Vector2 InputDirection { get; private set; }

    float radius;                                       //배경 반지름

    private void Start()
    {
        //sizeDelta = recttransform의 사이즈
        radius = background.sizeDelta.x * 0.5f;
    }

    //조이스틱 눌렀을때 발동
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    //조이스틱 드래그 할때 발동
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out pos);

        pos = Vector2.ClampMagnitude(pos, radius);

        handle.anchoredPosition = pos;

        InputDirection = pos / radius;
    }

    //조이스틱 땔때 발동
    public void OnPointerUp(PointerEventData eventData)
    {
        handle.anchoredPosition = Vector2.zero;
        InputDirection = Vector2.zero;
    }
}
