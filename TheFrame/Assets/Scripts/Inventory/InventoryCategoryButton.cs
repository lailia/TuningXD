using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryCategoryButton : MonoBehaviour
{
    public Image categoryBackground;
    public Image categoryImage;
    public Color defaultColor;
    public Color selectColor;

    public void SelectCategory()
    {
        categoryBackground.color = selectColor;
    }

    public void UnSelectCategory()
    {
        categoryBackground.color = defaultColor;
    }

}
