using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Inventory/NewItem")]
public class ItemData : ScriptableObject
{
    public int itemID;
    public string itemName;
    public Sprite itemIcon;
    public int itemNum;
    public string itemInfo;
    public bool isStack;

    public Action<ItemData> itemAction;
}
