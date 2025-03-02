using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUseManager : MonoBehaviour
{
    public Inventory myBag;
    public HotbarSlot[] hotbarSlot;
    public void SetupItemAction(ItemData item)
    {
        //根據itemID設定效果
        switch (item.itemID)
        {
            case 1: //1.HP Potion
                item.itemAction = (ItemData data) =>
                {
                    Debug.Log($"Use {data.itemName}");
                    //Player.instance.Heal(50);
                };
                break;
            case 2: //2.PP Potion
                item.itemAction = (ItemData data) =>
                {
                    Debug.Log($"Use {data.itemName}");
                    //Player.instance.RestoreMP(30);
                };
                break;
            case 3: //3.Power Up
                item.itemAction = (ItemData data) =>
                {
                    Debug.Log($"Use {data.itemName}");
                    //Player.instance.AddBuff("Attack", 10);
                };
                break;
            case 4: //4.Defense Up
                item.itemAction = (ItemData data) =>
                {
                    Debug.Log($"Use {data.itemName}");
                    //Player.instance.AddBuff("Attack", 10);
                };
                break;
            case 5: //5.Rebitrh
                item.itemAction = (ItemData data) =>
                {
                    Debug.Log($"Use {data.itemName}");
                    //Player.instance.AddBuff("Attack", 10);
                };
                break;
        }
    }
    public void UseItem(ItemData item)
    {
        if (item.itemAction == null) SetupItemAction(item);
        if (item == null) return;
        //確保這個道具有itemAction
        if (item.itemAction != null)
        {
            item.itemAction?.Invoke(item);
        }
        else
        {
            Debug.Log($"{item.itemName} 無法使用！");
        }
    }

    private void Update()
    {
        for (int i = 0; i < hotbarSlot.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)
                && hotbarSlot[i].slotItem != null && hotbarSlot[i].slotItem.itemNum > 0)
            {
                UseItem(hotbarSlot[i].slotItem);
                hotbarSlot[i].slotItem.itemNum -= 1;
                //當道具數量為0 清空
                if (hotbarSlot[i].slotItem.itemNum == 0)
                {
                    //保留hotbarSlot[i].slotItem資料以確保Remove()可以被正確執行
                    ItemData removedItem = hotbarSlot[i].slotItem;
                    hotbarSlot[i].slotItem = null;
                    myBag.itemList.Remove(removedItem);
                }
                InventoryManager.instance.RefreshUI();
            }
        }
    }
}
