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
    private Health health;
    private Coroutine hpCoroutine;
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

        Image image = bossPanel.GetComponent<Image>();
        Color color = image.color;

        while(timer < showTimer)
        {
            timer += Time.deltaTime;
            
            bossTitle.transform.position = Vector3.Lerp(originalVector3, bossHP.transform.position, timer/showTimer);
            color.a = Mathf.Lerp(1, 0, timer/showTimer);
            image.color = color;

            yield return null;
        }

        bossTitle.transform.position = bossHP.transform.position;

        StartCoroutine(ShowBossHP(0.4f));
    }

    private IEnumerator ShowBossHP(float showTimer)
    {
        bossPanel.gameObject.SetActive(false);
        bossTitle.gameObject.SetActive(false);

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

    public void SetHealth(Health health)
    {
        health.OnDamage += SetHP;
        health.OnDead += SetHP;
        this.health = health;
    }

    private void SetHP()
    {
        currentHP.fillAmount = health.GetHealthRatio();

        if(hpCoroutine != null)
        {
            StopCoroutine(hpCoroutine);
        }
        hpCoroutine = StartCoroutine(HPFade(0.7f));
    }

    private IEnumerator HPFade(float fadeTimer)
    {
        float timer = 0;

        Color color = costHP.color;

        while(timer < fadeTimer)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, timer/fadeTimer);

            costHP.color = color;

            yield return null;
        }

        costHP.fillAmount = currentHP.fillAmount;
    }
}
