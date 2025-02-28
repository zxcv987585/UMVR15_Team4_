using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Item slotItem;
    public Image itemImage;
    public TextMeshProUGUI itemNumText;
    public TextMeshProUGUI itemName;

    public void SetItem(Item newItem)
    {
        slotItem = newItem;
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if (slotItem != null)
        {
            itemImage.sprite = slotItem.itemIcon;
            itemImage.enabled = true;
            itemName.text = slotItem.name;

            //判斷道具是否為可堆疊，設定為可堆疊物件才顯示數量
            if (slotItem.isStack && slotItem.itemNum >= 0)
            {
                itemNumText.text = slotItem.itemNum.ToString();
                itemNumText.enabled = true;
            }
            else
            {
                itemNumText.enabled = false;
            }
        }
        else
        {
            //清空Slot的UI
            itemImage.sprite = null;
            itemImage.enabled = false;
            itemNumText.text = "";
            itemNumText.enabled = false;
            itemName.text = "-";
        }
    }
}
