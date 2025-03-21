using UnityEngine;
using UnityEngine.UI;

public class SkillListManager : MonoBehaviour
{
    public static SkillListManager instance;
    public SkillBag mySkill;
    public SkillSlot[] skillSlots;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        if (skillSlots == null || skillSlots.Length == 0)
        {
            skillSlots = GetComponentsInChildren<SkillSlot>();
        }

        foreach (var slot in skillSlots)
        {
            if (slot == null)
            {
                Debug.LogError("SkillListManager: skillSlots 中有 null 值！");
                continue;
            }

            slot.slotImage = slot.GetComponent<Image>();

            if (slot.slotImage == null)
            {
                Debug.LogError($"SkillListManager: Slot {slot.name} 沒有 Image 組件！");
            }
        }
    }

    private void Start()
    {
        foreach (var slot in skillSlots)
        {
            if (slot == null) continue;

            if (slot.slotImage == null)
            {
                slot.slotImage = slot.GetComponent<Image>();
            }
        }
        RefreshUI(); 
    }

    public void RefreshUI()
    {
        for (int i = 0; i < skillSlots.Length; i++)
        {
            skillSlots[i].SetSlotIndex(i);
            if (i < mySkill.skillList.Count)
            {
                skillSlots[i].skillData = mySkill.skillList[i];
            }
            else
            {
                skillSlots[i].skillData = null;
            }
            skillSlots[i].UpdateSkillListSlot();
        }
    }

    public void UnlockSkill(int index)
    {
        if (index >= 0 && index < mySkill.skillList.Count)
        {
            mySkill.skillList[index].isUnlocked = true;
            RefreshUI();
        }
    }
}

