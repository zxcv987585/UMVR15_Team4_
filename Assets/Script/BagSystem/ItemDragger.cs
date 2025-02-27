using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragger : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Canvas mainCanvas;
    private RectTransform mainCanvasRect;
    private Transform dragOriginParent = null;
    private Image dragImage = null;

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
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        PointerEventData pointerData = eventData;
        GameObject dragTarget = pointerData.pointerDrag;

        //如果物件Tag不等於Item則將eventData清空後return
        //確保後續的Drag EndDrag不會有物件可以執行
        if (!dragTarget.CompareTag("Item"))
        {
            eventData.pointerDrag = null;
            return;
        }

        //將拖曳物件的Image raycastTarget關閉，避免射線被此物件遮擋
        if (dragImage != null)
        {
            dragImage.raycastTarget = false;
        }

        dragOriginParent = dragTarget.transform.parent;
        //物件Parent移到Canvas下，避免被遮擋
        dragTarget.transform.SetParent(mainCanvas.transform);
        dragTarget.transform.localScale = dragTarget.transform.localScale * 1.2f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        PointerEventData pointerData = eventData;
        GameObject dragTarget = pointerData.pointerDrag;

        //如果是ScreenSpaceCamera的情況，老師寫的(X
        if (mainCanvas.renderMode == RenderMode.ScreenSpaceCamera && mainCanvas.worldCamera != null)
        {
            Vector2 vOut = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle
                (mainCanvasRect, Input.mousePosition, mainCanvas.worldCamera, out vOut);
            dragTarget.transform.localPosition = vOut;
        }
        else
        {
            dragTarget.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        PointerEventData pointerData = eventData;
        GameObject dragTarget = pointerData.pointerDrag;

        //判斷物件是否有移至其他容器
        if (dragTarget.transform.parent == mainCanvas.transform)
        {
            dragTarget.transform.SetParent(dragOriginParent);
        }

        dragTarget.transform.localPosition = Vector3.zero;
        dragTarget.transform.localScale = Vector3.one;

        //重新開啟raycastTarget
        if (dragImage != null)
        {
            dragImage.raycastTarget = true;
        }
    }
}
