using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Script初春い璶砆具笵ㄣ
public class ItemGet : MonoBehaviour
{
    //砆具笵ㄣㄤScriptableObject妮┦
    public ItemData thisItem;
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
