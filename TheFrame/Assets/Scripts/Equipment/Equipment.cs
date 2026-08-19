using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{
    public WeaponInstance mainWeapon;
    public ArmorInstance bodyArmor;

    public Sprite mainWeaponDefaultSprite;
    public Sprite bodyArmorDefaultSprite;

    [Header("Images")]
    public Image mainWeaponImage;
    public Image bodyArmorImage;

    [SerializeField]
    private Color baseColor = new Color(100f / 255f, 100f / 255f, 100f / 255f, 100f / 255f);
    [SerializeField]
    private Color activeColor = new Color(1f, 1f, 1f, 1f);

    public ItemInstance changeItem;

    public Player player;

    public void SetItem(ItemInstance item)
    {
        if (item is WeaponInstance weapon)
        {
            if (mainWeapon != null)
            {
                changeItem = mainWeapon;
            }

            mainWeapon = weapon;
            player.WeaponSet(mainWeapon);
        }
        else if (item is ArmorInstance armor)
        {
            if (bodyArmor != null)
            {
                changeItem = bodyArmor;
            }

            bodyArmor = armor;
            player.ArmorSet(bodyArmor);
        }
        ChangeImage();
    }

    public ItemInstance EquipChange()
    {
        if(changeItem.Data!=null)
        {
            ItemInstance item = changeItem;
            //changeItem = new ItemInstance();
            return item;
        }
        return null;
    }

    public void UnEquipItem(ItemInstance item)
    {
        if (item.Data.itemType==ItemType.weapon)
        {
            mainWeapon = null;
            mainWeaponImage.sprite = mainWeaponDefaultSprite;
            mainWeaponImage.color = baseColor;
            player.WeaponSet(mainWeapon);
        }
        else if (item.Data.itemType==ItemType.armor)
        {
            bodyArmor = null;
            bodyArmorImage.sprite = bodyArmorDefaultSprite;
            bodyArmorImage.color = baseColor;
            player.ArmorSet(bodyArmor);
        }
    }

    void Update()
    {
        
    }

    public void ChangeImage()
    {
        if(mainWeapon.weapon!=null)
        {
            mainWeaponImage.sprite = mainWeapon.weapon.icon;
            mainWeaponImage.color = activeColor;
        }
        if(bodyArmor.armor!=null)
        {
            bodyArmorImage.sprite = bodyArmor.armor.icon;
            bodyArmorImage.color = activeColor;
        }
    }

    public ItemInstance GetItem(int count)
    {
        ItemInstance item = null;

        if (count == -1)
        {
            item = mainWeapon;
        }
        else if (count == -2)
        {
            item = bodyArmor;
        }

        return item;
    }

}
