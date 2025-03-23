using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemUseManager : MonoBehaviour
{
    public Inventory myBag;
    public HotbarSlot[] hotbarSlotList;
    public RebirthUI rebirthUI;

    private PlayerController player;
    private PlayerHealth health;

    private Dictionary<GameInput.Bind, HotbarSlot> _itemBind;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        health = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        rebirthUI = GetComponentInChildren<RebirthUI>();

        rebirthUI.UseReviveItem += UseRivive;
        
        InitItemBind();
        GameInput.Instance.OnUseItem += CallBindItem;
    }

    private void InitItemBind()
    {
        if(hotbarSlotList.Length < 6)
        {
            Debug.Log("ItemUseManager 的 HotbarSlotList 數量少於 6 個");
            return;
        }

        _itemBind = new Dictionary<GameInput.Bind, HotbarSlot>
        {
            {GameInput.Bind.UseItem1, hotbarSlotList[0]},
            {GameInput.Bind.UseItem2, hotbarSlotList[1]},
            {GameInput.Bind.UseItem3, hotbarSlotList[2]},
            {GameInput.Bind.UseItem4, hotbarSlotList[3]},
            {GameInput.Bind.UseItem5, hotbarSlotList[4]},
            {GameInput.Bind.UseItem6, hotbarSlotList[5]},
        };
    }

    private void CallBindItem(GameInput.Bind bind)
    {
        if(!_itemBind.TryGetValue(bind, out HotbarSlot hotbarSlot)) return;
        if(hotbarSlot.slotItem == null || hotbarSlot.slotItem.itemNum <= 0) return;

        if(UseItem(hotbarSlot.slotItem))
        {
            hotbarSlot.slotItem.itemNum--;

            if(hotbarSlot.slotItem.itemNum == 0)
            {
                myBag.itemList.Remove(hotbarSlot.slotItem);
                hotbarSlot.slotItem = null;
            }

            InventoryManager.instance.RefreshUI();
        }
    }

    private void UseRivive()
    {
        ItemData reviveItem = myBag.itemList.Find(item => item.itemID == 5);
        if (reviveItem != null)
        {
            reviveItem.itemNum--;
            if (reviveItem.itemNum <= 0)
            {
                myBag.itemList.Remove(reviveItem);

                foreach (var slot in hotbarSlotList)
                {
                    if (slot.slotItem != null && slot.slotItem.itemID == 5)
                    {
                        slot.slotItem = null;
                    }
                }
            }
            InventoryManager.instance.RefreshUI();
        }
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
                    AudioManager.Instance.PlaySound("HPHealth", transform.position);
                };
                break;
            case 2: //2.PP Potion
                item.itemAction = (ItemData data) =>
                {
                    Debug.Log($"Use {data.itemName}");
                    health.HealPP(data.amount);
                    AudioManager.Instance.PlaySound("PPHealth", transform.position);
                };
                break;
            case 3: //3.Power Up
                item.itemAction = (ItemData data) =>
                {
                    Debug.Log($"Use {data.itemName}");
                    if(!player.IsAttackBuff)
                    StartCoroutine(player.AttackUP(data.amount, data.duration));
                    AudioManager.Instance.PlaySound("StatusUp", transform.position);
                };
                break;
            case 4: //4.Defense Up
                item.itemAction = (ItemData data) =>
                {
                    Debug.Log($"Use {data.itemName}");
                    if(!player.IsDefenseBuff)
                    StartCoroutine(player.DefenseUP(data.amount, data.duration));
                    AudioManager.Instance.PlaySound("StatusUp", transform.position);
                };
                break;
            case 5: //5.Rebitrh
                item.itemAction = (ItemData data) =>
                {
                    Debug.Log($"復活道具無法直接使用");
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
        if (item.itemID == 5 && !player.IsDie)
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
}
