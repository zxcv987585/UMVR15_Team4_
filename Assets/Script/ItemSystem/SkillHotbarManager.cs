using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class SkillHotbarManager : MonoBehaviour
{
    public static SkillHotbarManager instance;

    public List<SkillSlot> hotbarSlots;
    public SkillListManager skillListManager;

    private void Awake()
    {
        skillListManager = GetComponent<SkillListManager>();

        if (instance != null) Destroy(this);
        instance = this;

        foreach (var slot in hotbarSlots)
        {
            if (slot == null)
            {
                Debug.LogError("SkillHotbarManager: hotbarSlots 中有 null 值！");
                continue;
            }

            slot.slotImage = slot.GetComponent<Image>();

            if (slot.slotImage == null)
            {
                Debug.LogError($"SkillHotbarManager: Slot {slot.name} 沒有 Image 組件！");
            }
        }
    }

    private void Start()
    {
        //確保每個hotbar slot有正確的index
        for (int i = 0; i < hotbarSlots.Count; i++)
        {
            hotbarSlots[i].SetSlotIndex(i);
        }
    }

    public void AssignSkillToHotbar(SkillDataSO skill, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= hotbarSlots.Count) return;

        //檢查快捷欄是否已經有這個技能
        SkillSlot previousSlot = null;
        for (int i = 0; i < hotbarSlots.Count; i++)
        {
            if (hotbarSlots[i].skillData == skill)
            {
                //記錄這個HotbarSlot
                previousSlot = hotbarSlots[i];
                
                //清除原本快捷欄內的道具
                previousSlot.skillData = null;
                previousSlot.UpdateHotbarSlot();
                
                switch(i)
                {
                    case 0:
                        SkillManager.Instance.RemoveSkillBind(GameInput.Bind.Skill1);
                        break;
                    case 1:
                        SkillManager.Instance.RemoveSkillBind(GameInput.Bind.Skill2);
                        break;
                }
                
                break;
            }
        }

        /*
        if (previousSlot != null)
        {
            //清除原本快捷欄內的道具
            previousSlot.skillData = null;
            previousSlot.UpdateHotbarSlot();
        }
        */

        hotbarSlots[slotIndex].skillData = skill;
        hotbarSlots[slotIndex].UpdateHotbarSlot();
    }
    public void RefreshHotbarUI()
    {
        if (hotbarSlots == null || hotbarSlots.Count == 0)
        {
            Debug.LogError("SkillHotbarManager: hotbarSlots 未初始化！");
            return;
        }

        foreach (var slot in hotbarSlots)
        {
            if (slot == null)
            {
                Debug.LogError("SkillHotbarManager: 某個 hotbarSlot 為 null，跳過更新！");
                continue;
            }

            slot.UpdateHotbarSlot();
        }
    }
}
