using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemTake : MonoBehaviour
{
    //宣告被撿取的道具並指向其ScriptableObject屬性
    public Item thisItem;
    public Inventory playerInventory;
    public InventoryManager inventoryManager;

    //測試，模擬撿取
    private void OnMouseDown()
    {
        if (thisItem != null)
        {
            inventoryManager.AddItem(thisItem);
            
            //Destroy(gameObject); 
        }
    }
}
