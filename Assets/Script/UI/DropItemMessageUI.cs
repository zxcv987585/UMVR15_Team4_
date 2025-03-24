using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropItemMessageUI : MonoBehaviour
{
    [SerializeField][Header("消失時間")] private float _showTime = 1f;
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemText;
    [SerializeField] private List<Image> _otherImageList;
    
    public void SetItemInfo(ItemData itemData)
    {
        _itemImage.sprite = itemData.itemIcon;
        _itemText.text = itemData.itemName;

        StartCoroutine(MoveUpCoroutine(_showTime));
    }

    private IEnumerator MoveUpCoroutine(float showTime)
    {
        float timer = 0f;
        float newAlpha;
        Color originalColor;

        RectTransform rectTransform = transform as RectTransform;
        Vector2 originalOffsetMin = rectTransform.offsetMin; // 儲存原始 offsetMin
        Vector2 originalOffsetMax = rectTransform.offsetMax; // 儲存原始 offsetMax

        while(timer < showTime)
        {
            newAlpha = Mathf.Lerp(1, 0, timer/showTime);

            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, rectTransform.offsetMin.y + Time.deltaTime * 50f);
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, rectTransform.offsetMax.y + Time.deltaTime * 50f);

            _otherImageList.ForEach(img => {
                originalColor = img.color;
                originalColor.a = newAlpha;
                img.color = originalColor;
            });

            originalColor = _itemImage.color;
            originalColor.a = newAlpha;
            _itemImage.color = originalColor;

            originalColor = _itemText.color;
            originalColor.a = newAlpha;
            _itemText.color = originalColor;
            
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
