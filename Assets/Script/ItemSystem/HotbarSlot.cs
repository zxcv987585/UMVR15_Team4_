using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//代表背包內的一個slot 負責顯示myBag itemlist內對應的道具
//執行RefreshUI時，每個slot會更新UI
public class HotbarSlot : MonoBehaviour
{
    public ItemData slotItem;
    public Image slotImage;
    public TextMeshProUGUI slotNumtText;

    public void SetItem(ItemData newItem)
    {
        slotItem = newItem;
        slotImage.sprite = newItem != null ? newItem.itemIcon : null;
        slotNumtText.text = (newItem != null && newItem.isStack) ? newItem.itemNum.ToString() : "";
    }

    public void UpdateSlot()
    {
        if (slotItem != null)
        {
            slotImage.sprite = slotItem.itemIcon;
            slotImage.enabled = true;
            slotNumtText.text = slotItem.isStack ? slotItem.itemNum.ToString() : "";
        }
        else
        {
            slotImage.sprite = null;
            slotImage.enabled = false;
            slotNumtText.text = "";
        }
    }
    public void ClearSlot()
    {
        slotItem = null;
        slotImage.sprite = null;
        slotNumtText.text = "";
    }
}
