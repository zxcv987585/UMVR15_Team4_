using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<Slot> slots = new List<Slot>();
    public Inventory myBag;

    private void Start()
    {
        //初始化時刷新UI
        RefreshUI(); 
    }

    //道具被撿取後，從ItemTake那呼叫的方法
    public void AddItem(Item newItem)
    {
        //檢查背包內是否已經有這個道具
        foreach (Slot slot in slots)
        {
            //判斷Slot不是空的，而是有道具的
            //判斷Slot內的道具ID是否與撿取道具的ID一致
            //判斷道具是否可以推疊
            if (slot.slotItem != null && slot.slotItem.itemID == newItem.itemID && newItem.isStack)
            {
                slot.slotItem.itemNum += 1;
                slot.UpdateSlot();
                return;
            }
        }

        //找第一個空的Slot，將道具放進去
        foreach (Slot slot in slots)
        {
            //確保Slot是空的
            if (slot.slotItem == null) 
            {
                slot.SetItem(newItem);
                myBag.itemList.Add(newItem);
                slot.slotItem.itemNum += 1;
                slot.UpdateSlot();
                return;
            }
        }
        Debug.Log("背包滿了");
    }

    public void RefreshUI()
    {
        foreach (Slot slot in slots)
        {
            // 讓所有Slot更新UI
            slot.UpdateSlot(); 
        }
    }
}

