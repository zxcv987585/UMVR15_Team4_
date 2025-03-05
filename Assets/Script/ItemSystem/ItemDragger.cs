using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragger : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Canvas mainCanvas;
    private RectTransform mainCanvasRect;
    private Transform dragOriginParent = null;
    private Image dragImage = null;
    //紀錄原slot
    private Slot originSlot;
    private HotbarSlot originHotbarSlot;

    public Transform GetOriginalParent()
    {
        //保留原Parent標記作為Dropper的互換使用
         return dragOriginParent;
    }
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
        //取得當前的slot
        originSlot = GetComponentInParent<Slot>();
        originHotbarSlot = GetComponentInParent<HotbarSlot>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //道具欄為空且Tag不是item時不允許拖曳
        if ((originSlot == null || originSlot.slotItem == null) && (originHotbarSlot == null || originHotbarSlot.slotItem == null)) return;
        if (!transform.CompareTag("Item")) return;

        //將拖曳物件的Image raycastTarget關閉，避免射線被此物件遮擋
        //從而無法觸發Dropper判斷
        if (dragImage != null)
        {
            dragImage.raycastTarget = false;
        }

        //物件Parent移到Canvas下，避免被遮擋
        dragOriginParent = transform.parent;
        transform.SetParent(mainCanvas.transform);

        transform.localScale = transform.localScale * 1.2f;
        dragImage.color = new Color(dragImage.color.r*0.75f, dragImage.color.g * 0.75f, dragImage.color.b * 0.75f, dragImage.color.a);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //道具欄為空且Tag不是item時不允許拖曳
        if ((originSlot == null || originSlot.slotItem == null) && (originHotbarSlot == null || originHotbarSlot.slotItem == null)) return;
        if (!transform.CompareTag("Item")) return;

        //如果是ScreenSpaceCamera的情況，老師寫的(X
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
        //道具欄為空且Tag不是item時不允許拖曳
        if ((originSlot == null || originSlot.slotItem == null) && (originHotbarSlot == null)) return;
        if (!transform.CompareTag("Item")) return;

        transform.SetParent(dragOriginParent);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        dragImage.color = Color.white;

        //重新開啟raycastTarget
        if (dragImage != null)
        {
            dragImage.raycastTarget = true;
        }
    }

    public Slot GetOriginSlot()
    {
        // 取得原本的Slot
        return originSlot;
    }

    public ItemData GetItem()
    {
        if (originSlot != null)
        { 
            return originSlot?.slotItem;
        }
        else
        {
            return originHotbarSlot?.slotItem;
        }
    }
}
