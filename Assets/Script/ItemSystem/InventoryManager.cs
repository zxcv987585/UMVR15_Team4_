using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public Inventory myBag;
    public Slot[] slots;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        RefreshUI(); 
    }

    public void AddItem(ItemData newItem)
    {
        for (int i = 0; i < myBag.itemList.Count; i++)
        {
            if (myBag.itemList[i].itemID == newItem.itemID && newItem.isStack)
            {
                myBag.itemList[i].itemNum += 1;
                RefreshUI();
                return;
            }
        }

        if (myBag.itemList.Count < slots.Length)
        {
            myBag.itemList.Add(newItem);
            newItem.itemNum = 1;
            RefreshUI();
        }
        else
        {
            Debug.Log("�I�]�w��");
        }
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetSlotIndex(i);
            slots[i].UpdateSlot();
        }
        HotbarManager.instance.RefreshHotbarUI();
    }

    public void SwapItems(Slot slotA, Slot slotB)
    {
        if (slotA == null || slotB == null) return;

        int indexA = slotA.slotIndex;
        int indexB = slotB.slotIndex;

        if (indexA >= myBag.itemList.Count || indexB >= myBag.itemList.Count) return;
       
        ItemData temp = myBag.itemList[indexA];
        myBag.itemList[indexA] = myBag.itemList[indexB];
        myBag.itemList[indexB] = temp;
        
        this.PlaySound("EquipSkill");

        RefreshUI();
    }
}