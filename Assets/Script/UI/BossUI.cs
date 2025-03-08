using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    //-----開場動畫------------------------------------
    [SerializeField] private RectTransform bossPanel;
    [SerializeField] private RectTransform bossTitle;
    [SerializeField] private RectTransform bossName;
    [SerializeField] private RectTransform bossHP;
    //-------------------------------------------------

    //-----HP------------------------------------------
    [SerializeField] private Image currentHP;
    [SerializeField] private Image costHP;
    //-------------------------------------------------

    private void Start()
    {
        StartCoroutine(ShowBossPanel(0.7f));
    }

    private IEnumerator ShowBossPanel(float showTimer)
    {
        float timer = 0f;

        while(timer < showTimer)
        {
            timer += Time.deltaTime;
            bossPanel.localScale = new Vector3(Mathf.Lerp(0f, 1f, timer/showTimer), 1f, 1f);
            bossTitle.localScale = new Vector3(Mathf.Lerp(0f, 1f, timer/showTimer), 1f, 1f);

            yield return null;
        }

        bossPanel.localScale = Vector3.one;

        StartCoroutine(MoveBossTitle(0.7f));
    }

    private IEnumerator MoveBossTitle(float showTimer)
    {
        float timer = 0f;
        Vector3 originalVector3 = bossTitle.transform.position;

        while(timer < showTimer)
        {
            timer += Time.deltaTime;
            bossTitle.transform.position = Vector3.Lerp(originalVector3, bossHP.transform.position, timer/showTimer);

            yield return null;
        }

        bossTitle.transform.position = bossHP.transform.position;

        StartCoroutine(ShowBossHP(0.4f));
    }

    private IEnumerator ShowBossHP(float showTimer)
    {
        float timer = 0f;

        while(timer < showTimer)
        {
            timer += Time.deltaTime;
            bossName.localScale = new Vector3(Mathf.Lerp(0f, 1f, timer/showTimer), 1f, 1f);
            bossHP.localScale = new Vector3(Mathf.Lerp(0f, 1f, timer/showTimer), 1f, 1f);

            yield return null;
        }

        bossPanel.localScale = Vector3.one;
    }

    private void SetHP(float hpRatio)
    {
        currentHP.fillAmount = hpRatio;
    }
}
