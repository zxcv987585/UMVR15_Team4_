using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropItemMessageUI : MonoBehaviour
{
    [SerializeField][Header("消失時間")] private float _showTime = 0.75f;
    [SerializeField] private Image _itemImage;
    [SerializeField] private Image _bgImage;
    [SerializeField] private TextMeshProUGUI _itemText;
    [SerializeField] private List<Image> _otherImageList;

    private static float _offsetY = 75f; // 每個訊息間距
    private static List<DropItemMessageUI> _activeMessages = new List<DropItemMessageUI>();

    private void Start()
    {
        // 根據當前訊息數量，往上移動新生成的訊息
        RectTransform rectTransform = transform as RectTransform;
        rectTransform.anchoredPosition += new Vector2(0, _activeMessages.Count * _offsetY);

        _activeMessages.Add(this); // 記錄當前訊息
    }

    public void SetItemInfo(ItemData itemData)
    {
        _itemImage.sprite = itemData.itemIcon;
        _itemText.text = itemData.itemName;

        StartCoroutine(MoveUpCoroutine(_showTime));
    }

    private IEnumerator MoveUpCoroutine(float showTime)
    {
        yield return new WaitForSeconds(0.25f); // 先停留 0.25 秒

        float timer = 0f;
        float newAlpha;
        float newAlphaForBg;
        Color originalColor;

        RectTransform rectTransform = transform as RectTransform;

        while (timer < showTime)
        {
            newAlpha = Mathf.Lerp(1, 0, timer / showTime);
            newAlphaForBg = Mathf.Lerp(160 / 255f, 0, timer / showTime);

            rectTransform.anchoredPosition += new Vector2(0, Time.deltaTime * 50f);

            _otherImageList.ForEach(img => {
                originalColor = img.color;
                originalColor.a = newAlpha;
                img.color = originalColor;
            });

            // 修正 bgImage 透明度處理
            originalColor = _bgImage.color;
            originalColor.a = newAlphaForBg;
            _bgImage.color = originalColor;

            originalColor = _itemImage.color;
            originalColor.a = newAlpha;
            _itemImage.color = originalColor;

            originalColor = _itemText.color;
            originalColor.a = newAlpha;
            _itemText.color = originalColor;

            timer += Time.deltaTime;
            yield return null;
        }

        _activeMessages.Remove(this); // 從列表移除已消失的訊息
        Destroy(gameObject);
    }
}
