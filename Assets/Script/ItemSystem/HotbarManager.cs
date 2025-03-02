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
    public void AssignItemToHotbar(Item item, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= hotbarSlots.Count) return;
        hotbarSlots[slotIndex].SetItem(item);
    }
}
