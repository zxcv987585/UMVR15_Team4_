using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class HotbarManager : MonoBehaviour
{
    public static HotbarManager instance;

    public List<HotbarSlot> hotbarSlots;
    public Inventory myBag;

    private void Awake()
    {
        if (instance != null) Destroy(this);
        instance = this;
    }
    public void AssignItemToHotbar(ItemData item, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= hotbarSlots.Count) return;

        //搜尋丟至快捷欄裡的道具是否已有重複
        HotbarSlot previousSlot = null;
        for (int i = 0; i < hotbarSlots.Count; i++)
        {
            if (hotbarSlots[i].slotItem == item) 
            {
                //記錄這個HotbarSlot
                previousSlot = hotbarSlots[i]; 
                break;
            }
        }

        if (previousSlot != null)
        {
            //清除原本快捷欄內的道具
            previousSlot.ClearSlot();
        }

        hotbarSlots[slotIndex].SetItem(item);

        RefreshHotbarUI();
    }
    public void RefreshHotbarUI()
    {
        for (int i = 0; i < hotbarSlots.Count; i++)
        {
            hotbarSlots[i].UpdateSlot();
        }
    }
}
