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
                    if(!player.IsAttackBuff)
                    StartCoroutine(player.AttackUP(data.amount, data.duration));
                };
                break;
            case 4: //4.Defense Up
                item.itemAction = (ItemData data) =>
                {
                    Debug.Log($"Use {data.itemName}");
                    if(!player.IsDefenseBuff)
                        StartCoroutine(player.DefenseUP(data.amount, data.duration));
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
    public bool UseItem(ItemData item)
    {
        if (item == null) return false;
        //itemAction
        if (item.itemID == 3 && player.IsAttackBuff)
        {
            Debug.Log("玩家正在進入Buff狀態！");
            return false;
        }
        if (item.itemID == 4 && player.IsDefenseBuff)
        {
            Debug.Log("玩家正在進入Buff狀態！");
            return false;
        }

        if (item.itemAction == null) SetupItemAction(item);

        if (item.itemAction != null)
        {
            item.itemAction?.Invoke(item);
            return true;
        }

        item.itemAction?.Invoke(item);
        return true;
    }

    private void Update()
    {
        for (int i = 0; i < hotbarSlot.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)
                && hotbarSlot[i].slotItem != null && hotbarSlot[i].slotItem.itemNum > 0)
            {
                if (UseItem(hotbarSlot[i].slotItem))
                {
                    hotbarSlot[i].slotItem.itemNum -= 1;
                    //成功使用道具才減少數量
                    if (hotbarSlot[i].slotItem.itemNum == 0)
                    {
                        ItemData removedItem = hotbarSlot[i].slotItem;
                        hotbarSlot[i].slotItem = null;
                        myBag.itemList.Remove(removedItem);
                    }
                    InventoryManager.instance.RefreshUI();
                }
            }
        }
    }
}
