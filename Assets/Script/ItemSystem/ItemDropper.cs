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

        //判斷拖曳目標是否為快捷欄且物件是否為Item
        if (transform.CompareTag("HotbarSlots") && dragger.gameObject.CompareTag("Item"))
        {
            int slotIndex = transform.GetSiblingIndex();
            if(transform.parent.name == "slot_groupR")
            {
                slotIndex += 3;
            }
            HotbarManager.instance.AssignItemToHotbar(dragger.GetItem(), slotIndex);
        }
        else
        {
            Slot targetSlot = GetComponent<Slot>();
            InventoryManager.instance.SwapItems(dragger.GetOriginSlot(), targetSlot);
        }
    }
}
