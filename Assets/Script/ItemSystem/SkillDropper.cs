using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

        

        //�u���\�ޯ�즲��SkillHotbar
        if (transform.CompareTag("SkillHotbar") && dragger.gameObject.CompareTag("SkillList"))
        {
            //�Φ���k�d���ޭȡA�קKHierarchy�Ƴ\�y�����ޭȿ��ê����D�I�I�I
            int slotIndex = SkillHotbarManager.instance.hotbarSlots.IndexOf(GetComponent<SkillSlot>());

            //���o�ޯ઺weapon�W��
            Debug.Log(dragger.GetOriginSlot().GetComponent<SkillSlot>().skillData.weapon);
            SkillHotbarManager.instance.AssignSkillToHotbar(dragger.GetSkill(), slotIndex);
            SkillHotbarManager.instance.RefreshHotbarUI();
            
            switch(targetSlot.slotIndex)
            {
                case 0:
                    SkillManager.Instance.SetSkillBind(GameInput.Bind.Skill1, dragger.GetSkill());
                    break;
                case 1:
                    SkillManager.Instance.SetSkillBind(GameInput.Bind.Skill2, dragger.GetSkill());
                    break;
                default:
                    Debug.Log(" SkillDropper �b switch targetSlot.slotIndex �X�{���~");
                    break;
            }
        }
    }
}
