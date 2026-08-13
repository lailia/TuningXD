using UnityEngine;
using UnityEngine.EventSystems;

public class Buttons : MonoBehaviour,
    IPointerDownHandler,                    //누를때 이벤트
    IPointerUpHandler                       //땔때 이벤트
{
    public bool isButtonDown { get; private set; }

    public bool button;
    public bool toggle;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button && !toggle)
        {
            isButtonDown = true;
        }
        else if (!button&&toggle)
        {
            if (isButtonDown)
            {
                isButtonDown = false;
            }
            else
            {
                isButtonDown = true;
            }
        }
    }

    public void OnPointerUp(PointerEventData evenData)
    {
        if(button&&!toggle)
        {
            isButtonDown = false;
        }
    }

}
