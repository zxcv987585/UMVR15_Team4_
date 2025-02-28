using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemTake : MonoBehaviour
{
    //砆具笵ㄣㄤScriptableObject妮┦
    public Item thisItem;
    public Inventory playerInventory;
    public InventoryManager inventoryManager;

    //代刚家览具
    private void OnMouseDown()
    {
        if (thisItem != null)
        {
            inventoryManager.AddItem(thisItem);
            
            //Destroy(gameObject); 
        }
    }
}
