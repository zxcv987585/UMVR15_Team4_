using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//代阜囗包內的一個slot 負責顯示myBag itemlist內?E釭犒D?E
//執行RefreshUI時，每個slot會更新UI
public class HotbarSlot : MonoBehaviour
{
    public ItemData slotItem;
    public Image ItemImage;
    public Image slotImage;
    public Sprite nullImage;
    public TextMeshProUGUI slotNumtText;

    private void Awake()
    {
        // 確保 ItemImage 和 slotNumText 被正確綁定
        if (ItemImage == null)
        {
            Debug.LogWarning("HotbarSlot: ItemImage is NULL! 嘗試重新獲取...");
            ItemImage = GetComponent<Image>(); // 嘗試從當前物件獲取
        }

        if (slotNumtText == null)
        {
            Debug.LogWarning("HotbarSlot: slotNumText is NULL! 嘗試重新獲取...");
            slotNumtText = GetComponentInChildren<TextMeshProUGUI>(); // 從子物件獲取
        }
        nullImage = GetComponent<Image>().sprite;
    }

    public void SetItem(ItemData newItem)
    {
        slotItem = newItem;

        if (ItemImage == null)
        {
            Debug.LogError("HotbarSlot: ItemImage 仍為 NULL，請檢查 UI 設置！");
            return;
        }
        else
        {
            this.PlaySound("EquipSkill");
        }
        ItemImage.sprite = newItem != null ? newItem.itemIcon : null;
        slotNumtText.text = (newItem != null && newItem.isStack) ? newItem.itemNum.ToString() : "";
    }

    public void UpdateSlot()
    {
        if (slotItem != null)
        {
            slotImage.sprite = slotItem.itemIcon;
            ItemImage.sprite = slotItem.itemIcon;
            ItemImage.enabled = true;
            slotNumtText.text = slotItem.isStack ? slotItem.itemNum.ToString() : "";
        }
        else
        {
            slotImage.sprite = nullImage;
            ItemImage.sprite = null;
            ItemImage.enabled = false;
            slotNumtText.text = "";
        }
    }
    public void ClearSlot()
    {
        slotItem = null;
        ItemImage.sprite = null;
        slotNumtText.text = "";
    }
}
