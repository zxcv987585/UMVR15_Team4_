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

        // 確保 skillSlots 內的 Image 初始化
        foreach (var slot in skillSlots)
        {
            if (slot != null)
            {
                slot.slotImage = slot.GetComponent<Image>();
            }
        }
    }

    private void Start()
    {
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

