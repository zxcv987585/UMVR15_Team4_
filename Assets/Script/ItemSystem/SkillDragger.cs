using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillDragger : MonoBehaviour, IBeginDragHandler,IDragHandler ,IEndDragHandler
{
    private Canvas mainCanvas;
    private RectTransform mainCanvasRect;
    private Transform dragOriginParent = null;
    private Image dragImage = null;
    private SkillSlot originSlot;

    void Awake()
    {
        if (mainCanvas == null)
        {
            mainCanvas = GetComponentInParent<Canvas>();
        }
        if (mainCanvas != null)
        {
            mainCanvasRect = mainCanvas.GetComponent<RectTransform>();
        }
        dragImage = GetComponent<Image>();
        originSlot = GetComponentInParent<SkillSlot>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //如果沒有技能數據或SkillSlot是空的，則return
        if (originSlot == null || originSlot.skillData == null) return;

        // 限制只能從技能列表拖曳
        if (!originSlot.CompareTag("SkillList")) return;

        if (dragImage != null)
        {
            dragImage.raycastTarget = false;
        }

        dragOriginParent = transform.parent;
        transform.SetParent(mainCanvas.transform);
        transform.localScale = transform.localScale * 1.2f;
        dragImage.color = new Color(dragImage.color.r * 0.75f, dragImage.color.g * 0.75f, dragImage.color.b * 0.75f, dragImage.color.a);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (originSlot == null || originSlot.skillData == null) return;

        if (mainCanvas.renderMode == RenderMode.ScreenSpaceCamera && mainCanvas.worldCamera != null)
        {
            Vector2 vOut = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle
                (mainCanvasRect, Input.mousePosition, mainCanvas.worldCamera, out vOut);
            transform.localPosition = vOut;
        }
        else
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (originSlot == null || originSlot.skillData == null) return;

        transform.SetParent(dragOriginParent);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        dragImage.color = Color.white;

        if (dragImage != null)
        {
            dragImage.raycastTarget = true;
        }
    }

    public SkillSlot GetOriginSlot()
    {
        return originSlot;
    }

    public SkillDataSO GetSkill()
    {
        return originSlot?.skillData;
    }
}
