using UnityEngine;

public class SkillListManager : MonoBehaviour
{
    public static SkillListManager instance;
    public SkillBag mySkill;
    public SkillSlot[] skillSlots;

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

