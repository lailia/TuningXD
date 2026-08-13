using System;
using UnityEngine;
using UnityEngine.UI;

public class AttachmentSlot : MonoBehaviour
{
    public Sprite baseSprite;
    public Image imageComponent;
    public AttachmentInstance itemInstance;
    public Button button;
    public int index;

    [SerializeField]
    private Color baseColor = new Color(100f / 255f, 100f / 255f, 100f / 255f, 100f / 255f);

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Init(Sprite _baseSprite, int _index, Action<AttachmentInstance, bool> clickSlot)
    {
        baseSprite = _baseSprite;
        index = _index;
        button.onClick.AddListener(() => clickSlot(itemInstance, true));
    }

    public void NewImageChange(AttachmentInstance instance)
    {
        itemInstance = instance;
        imageComponent.sprite = instance.attachment.icon;
        imageComponent.color = new Color(1f, 1f, 1f, 1f);
    }

    public void BaseImageChange()
    {
        itemInstance = null;
        imageComponent.sprite = baseSprite;
        imageComponent.color = baseColor;
    }

}
