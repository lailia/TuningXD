using System.Collections.Generic;
using UnityEngine;

public class InventoryCategoryUI : MonoBehaviour
{
    public List<InventoryCategoryButton> categoryButtons = new List<InventoryCategoryButton>();
    public int categoryIndex;

    public void HandleSlotClick(int index)
    {
        categoryButtons[categoryIndex].UnSelectCategory();
        categoryButtons[index].SelectCategory();
        categoryIndex = index;
    }
}
