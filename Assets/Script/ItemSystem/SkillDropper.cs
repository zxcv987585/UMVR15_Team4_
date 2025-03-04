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
            int slotIndex = transform.GetSiblingIndex();
            SkillHotbarManager.instance.AssignSkillToHotbar(dragger.GetSkill(), slotIndex);
        }
    }
}
