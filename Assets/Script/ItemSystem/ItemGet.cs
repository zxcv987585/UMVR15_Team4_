using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Script初春い璶砆具笵▉E
public class ItemGet : MonoBehaviour
{
    //砆具笵ㄣ珌EVㄤScriptableObject妮┦
    public ItemData thisItem;
    public Inventory playerInventory;
    public InventoryManager inventoryManager;

    //代刚家览具▉E
    private void OnMouseDown()
    {
        if (thisItem != null)
        {
            inventoryManager.AddItem(thisItem);
            
            //Destroy(gameObject); 
        }
    }
}
