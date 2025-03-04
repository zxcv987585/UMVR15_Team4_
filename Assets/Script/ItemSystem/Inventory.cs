using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Inventory/NewInventory")]
public class Inventory : ScriptableObject
{
    public List<ItemData> itemList = new List<ItemData>();
}
