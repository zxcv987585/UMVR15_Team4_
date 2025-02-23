using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOptionSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonRepeatMessageText;
    [SerializeField] private TextMeshProUGUI keyboardText;
    [SerializeField] private Button button;
    [SerializeField] private GameInput.Bind bind;

    private Coroutine fadeCoroutine;

    private void Start()
    {
        UpdateVisual();

        button.onClick.AddListener(() => 
        {
            keyboardText.text = "...";
            GameInput.Instance.ReBindKeyboard(bind, UpdateVisual, ShowFailReBindMessage);
        });
    }

    /// <summary>
    /// 更新設定頁面中, 顯示的綁定按鍵文字
    /// </summary>
    private void UpdateVisual()
    {
        keyboardText.text = GameInput.Instance.GetBindText(bind);
    }

    private void ShowFailReBindMessage()
    {
        if(fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        
        fadeCoroutine = StartCoroutine(FailRebindMessage_FadeOut());
    }

    private IEnumerator FailRebindMessage_FadeOut(float showTimer = 2f)
    {
        buttonRepeatMessageText.gameObject.SetActive(true);
        buttonRepeatMessageText.alpha = 1f;
        yield return new WaitForSeconds(showTimer);

        float fadeTimer = 1f;
        float timer = 0f;

        while(timer < fadeTimer)
        {
            timer += Time.deltaTime;
            buttonRepeatMessageText.alpha = Mathf.Lerp(1f , 0f, timer / fadeTimer);

            yield return null;
        }

        buttonRepeatMessageText.gameObject.SetActive(false);
        fadeCoroutine = null;
    }

}
