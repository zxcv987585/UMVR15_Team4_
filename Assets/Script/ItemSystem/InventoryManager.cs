using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

//負責將道具加進itemlist
//並用RefreshUI負責將itemlist內的道具顯示在slot上
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public Inventory myBag;
    public Slot[] slots;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        //初始化時刷新UI
        RefreshUI(); 
    }

    //道具被撿取後，從ItemGet那呼叫的方法
    public void AddItem(Item newItem)
    {
        //檢查myBag itemlist看看這個道具是否已存在
        for (int i = 0; i < myBag.itemList.Count; i++)
        {
            //判斷Slot內的道具ID是否與撿取道具的ID一致
            //判斷道具是否可以推疊
            if (myBag.itemList[i].itemID == newItem.itemID && newItem.isStack)
            {
                myBag.itemList[i].itemNum += 1;
                RefreshUI();
                return;
            }
        }

        //如果myBag itemList裡沒有這個道具或者不可堆疊
        //且背包沒有滿就新增道具
        if (myBag.itemList.Count < slots.Length)
        {
            myBag.itemList.Add(newItem);
            newItem.itemNum = 1;
            RefreshUI();
        }
        else
        {
            Debug.Log("背包已滿");
        }
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            //讓每格slot呼叫方法SetSlotIndex抓取索引並讀取資料更新UI
            slots[i].SetSlotIndex(i);
            slots[i].UpdateSlot();
        }
    }

    public void SwapItems(Slot slotA, Slot slotB)
    {
        if (slotA == null || slotB == null) return;

        int indexA = slotA.slotIndex;
        int indexB = slotB.slotIndex;

        if (indexA >= myBag.itemList.Count || indexB >= myBag.itemList.Count) return;
       
        // 交換 mybag itemlist 內的 itemData
        Item temp = myBag.itemList[indexA];
        myBag.itemList[indexA] = myBag.itemList[indexB];
        myBag.itemList[indexB] = temp;

        RefreshUI();
    }
}

