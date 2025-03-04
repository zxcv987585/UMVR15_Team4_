using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

//負責將道具加進itemlist
//並用RefreshUI負責將itemlist內的道具顯示在slot上
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
        //初始化時刷新UI
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

