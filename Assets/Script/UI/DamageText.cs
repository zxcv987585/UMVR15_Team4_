using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float showTime;
    [SerializeField] private TextMeshProUGUI damageText;

    public void Show(float damage)
    {
        gameObject.SetActive(true);
        damageText.text = damage.ToString();
        StartCoroutine(ShowAnimationCoroutine());
    }

    private IEnumerator ShowAnimationCoroutine()
    {
        float timer = 0f;

        while(timer < showTime)
        {
            timer += Time.deltaTime;

            //文字上浮效果
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;

            //文字淡出效果
            damageText.alpha = Mathf.Lerp(1f, 0f, timer/showTime);

            yield return null;
        }

        gameObject.SetActive(false);
        BattleUIManager.Instance.RecycleDamageText(this);
    }
}
