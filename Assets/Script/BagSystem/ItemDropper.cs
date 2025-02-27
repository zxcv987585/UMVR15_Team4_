using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDropper : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData == null) { return; }
        PointerEventData pointerData = eventData;
        GameObject dragTarget = pointerData.pointerDrag;
        if (dragTarget == null) { return; }
        if (!dragTarget.CompareTag("Item"))
        {
            return;
        }

        //先找尋slot內是否有掛ItemDragger的物件
        Transform currentChild = null;
        foreach (Transform child in transform)
        {
            if (child.GetComponent<ItemDragger>() != null)
            {
                currentChild = child;
                break;
            }
        }

        //////---如果slot已有物件---
        ////複蓋
        //if (currentChild != null)
        //{
        //    Destroy(currentChild.gameObject);
        //}

        //互換
        if (currentChild != null)
        {
            Transform originalParent = dragTarget.GetComponent<ItemDragger>().GetOriginalParent();
            currentChild.SetParent(originalParent);
            currentChild.transform.localPosition = Vector3.zero;
            currentChild.transform.localScale = Vector3.one;
        }

        dragTarget.transform.SetParent(transform);
        dragTarget.transform.localPosition = Vector3.zero;
        dragTarget.transform.localScale = Vector3.one;
    }
}
