using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillDropper : MonoBehaviour, IDropHandler
{
    private SkillSlot targetSlot;

    void Awake()
    {
        targetSlot = GetComponent<SkillSlot>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        SkillDragger dragger = eventData.pointerDrag.GetComponent<SkillDragger>();
        if (dragger == null) return;

        //只允許技能拖曳到SkillHotbar
        if (transform.CompareTag("SkillHotbar") && dragger.gameObject.CompareTag("SkillList"))
        {
            //用此方法查索引值，避免Hierarchy排許造成索引值錯亂的問題！！！
            int slotIndex = SkillHotbarManager.instance.hotbarSlots.IndexOf(GetComponent<SkillSlot>());
            
            SkillHotbarManager.instance.AssignSkillToHotbar(dragger.GetSkill(), slotIndex);
            SkillHotbarManager.instance.RefreshHotbarUI();
        }
    }
}
