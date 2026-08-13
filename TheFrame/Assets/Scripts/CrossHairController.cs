using UnityEngine;

public class CrossHairController : MonoBehaviour
{
    public RectTransform[] crossHairs;              //0 : up, 1 : down, 2 : ringht, 3 : left

    private float currentSpread;

    void Start()
    {
        
    }

    void Update()
    {
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
}
