using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//代表背包內的一個slot 負責顯示myBag itemlist內對應的道具
//執行RefreshUI時，每個slot會更新UI
public class Slot : MonoBehaviour
{   
    //slot在 myBag item list 對應的索引
    public int slotIndex;
    private Item slotItem;
    public Image itemImage;
    public TextMeshProUGUI itemNumText;
    public TextMeshProUGUI itemName;

    public void SetSlotIndex(int index)
    {
        //設定該slot對應myBag item List的哪一格
        slotIndex = index;
    }

    //更新slot的道具名稱、數量等
    public void UpdateSlot()
    {
        InventoryManager inventoryManager = InventoryManager.instance;

        //確保 myBag 內有這個索引
        if (inventoryManager.myBag.itemList.Count > slotIndex)
        {
            //抓取 mybag item list 的數值
            slotItem = inventoryManager.myBag.itemList[slotIndex];

            itemImage.sprite = slotItem.itemIcon;
            itemImage.enabled = true;
            itemName.text = slotItem.itemName;

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
