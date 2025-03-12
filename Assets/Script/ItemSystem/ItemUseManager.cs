using UnityEngine;

public class ItemUseManager : MonoBehaviour
{
    public Inventory myBag;
    public HotbarSlot[] hotbarSlot;

    private PlayerController player;
    private PlayerHealth health;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        health = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }
    public void SetupItemAction(ItemData item)
    {
        //檢查道具欄位
        switch (item.itemID)
        {
            case 1: //1.HP Potion
                item.itemAction = (ItemData data) =>
                {
                    Debug.Log($"Use {data.itemName}");
                    health.Heal(data.amount);
                };
                break;
            case 2: //2.PP Potion
                item.itemAction = (ItemData data) =>
                {
                    Debug.Log($"Use {data.itemName}");
                    health.HealPP(data.amount);
                };
                break;
            case 3: //3.Power Up
                item.itemAction = (ItemData data) =>
                {
                    Debug.Log($"Use {data.itemName}");
                    //Player.instance.AddBuff("Attack", 10);
                };
                break;
            case 4: //4.Defense Up
                item.itemAction = (ItemData data) =>
                {
                    Debug.Log($"Use {data.itemName}");
                    //Player.instance.AddBuff("Attack", 10);
                };
                break;
            case 5: //5.Rebitrh
                item.itemAction = (ItemData data) =>
                {
                    Debug.Log($"Use {data.itemName}");
                    //Player.instance.AddBuff("Attack", 10);
                };
                break;
        }
    }
    public void UseItem(ItemData item)
    {
        if (item.itemAction == null) SetupItemAction(item);
        if (item == null) return;
        //itemAction
        if (item.itemAction != null)
        {
            item.itemAction?.Invoke(item);
        }
        else
        {
            Debug.Log($"{item.itemName} ｵLｪkｨﾏ･ﾎ｡I");
        }
    }

    private void Update()
    {
        for (int i = 0; i < hotbarSlot.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)
                && hotbarSlot[i].slotItem != null && hotbarSlot[i].slotItem.itemNum > 0)
            {
                UseItem(hotbarSlot[i].slotItem);
                hotbarSlot[i].slotItem.itemNum -= 1;
                //ｷ晥Dｨ羮ﾆｶqｬｰ0 ｲMｪﾅ
                if (hotbarSlot[i].slotItem.itemNum == 0)
                {
                    //ｫOｯdhotbarSlot[i].slotItemｸ・ﾆ･HｽTｫORemove()･i･HｳQ･ｿｽTｰ・
                    ItemData removedItem = hotbarSlot[i].slotItem;
                    hotbarSlot[i].slotItem = null;
                    myBag.itemList.Remove(removedItem);
                }
                InventoryManager.instance.RefreshUI();
            }
        }
    }
}
