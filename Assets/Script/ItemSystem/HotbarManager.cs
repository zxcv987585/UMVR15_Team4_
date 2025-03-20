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
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        // 確保 HotbarSlots 是從 UI 正確綁定的
        if (hotbarSlots.Count == 0)
        {
            Debug.LogWarning("HotbarManager: 嘗試自動獲取 HotbarSlot...");
            hotbarSlots.AddRange(GetComponentsInChildren<HotbarSlot>());
        }

        foreach (var slot in hotbarSlots)
        {
            if (slot == null)
            {
                Debug.LogError("HotbarManager: 有 HotbarSlot 為 NULL，請檢查 UI 設置！");
            }
        }
    }

    public void AssignItemToHotbar(ItemData item, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= hotbarSlots.Count)
        {
            Debug.LogError($"HotbarManager: 指定的 HotbarSlot 超出範圍 ({slotIndex})！");
            return;
        }

        if (hotbarSlots[slotIndex] == null)
        {
            Debug.LogError($"HotbarManager: HotbarSlot at index {slotIndex} is missing!");
            return;
        }
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
            //清除
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
