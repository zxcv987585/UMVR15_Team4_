using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDropper : MonoBehaviour, IDropHandler
{
    private Slot targetSlot;

    void Awake()
    {
        //取得該Dropper的slot
        targetSlot = GetComponent<Slot>(); 
    }

    public void OnDrop(PointerEventData eventData)
    {
        //確保拖曳物品存在
        ItemDragger dragger = eventData.pointerDrag.GetComponent<ItemDragger>();
        if (dragger == null) return;

        if (transform.CompareTag("HotbarSlots"))
        {
            int slotIndex = transform.GetSiblingIndex();
            HotbarManager.instance.AssignItemToHotbar(dragger.GetItem(), slotIndex);
        }
        else
        {
            Slot targetSlot = GetComponent<Slot>();
            InventoryManager.instance.SwapItems(dragger.GetOriginSlot(), targetSlot);
        }

        ////取得拖曳物品的slot並確保slot存在
        //Slot originSlot = dragger.GetOriginSlot(); 
        //if (originSlot == null || targetSlot == null) return;

        ////交換 mybag itemlist 內的數據
        //InventoryManager inventoryManager = InventoryManager.instance;
        //if (inventoryManager == null) return;

        //int originIndex = originSlot.slotIndex;
        //int targetIndex = targetSlot.slotIndex;

        //if (inventoryManager.myBag.itemList.Count > originIndex && inventoryManager.myBag.itemList.Count > targetIndex)
        //{
        //    // 交換 mybag itemlist 內的 itemData
        //    Item tempItem = inventoryManager.myBag.itemList[originIndex];
        //    inventoryManager.myBag.itemList[originIndex] = inventoryManager.myBag.itemList[targetIndex];
        //    inventoryManager.myBag.itemList[targetIndex] = tempItem;

        //    //更新UI
        //    inventoryManager.RefreshUI();
        //}
    }
}
