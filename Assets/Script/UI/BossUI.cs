using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    //-----開場動畫------------------------------------
    [SerializeField] private RectTransform bossPanel;
    [SerializeField] private RectTransform bossTitle;
    [SerializeField] private Image bossTitleBar;
    [SerializeField] private RectTransform bossSubTitle;
    [SerializeField] private RectTransform bossName;
    [SerializeField] private RectTransform bossHP;
    [SerializeField] private RectTransform flash;
    //-------------------------------------------------

    //-----HP------------------------------------------
    private Health health;
    private Coroutine hpCoroutine;
    [SerializeField] private Image currentHP;
    [SerializeField] private Image costHP;
    //-------------------------------------------------

    private void Start()
    {
        StartCoroutine(ShowBossPanel(1.5f));
    }

    private IEnumerator ShowBossPanel(float showTimer)
    {
        EasyInOut easyInOut = FindObjectOfType<EasyInOut>();

        //BossTitle出現動畫
        bossTitle.gameObject.SetActive(true);
        StartCoroutine(easyInOut.ChangeValue(0f, 28f, 1.5f,
           value => bossTitle.GetComponent<TextMeshProUGUI>().characterSpacing = value,
           EasyInOut.EaseOut));
        StartCoroutine(easyInOut.ChangeValue(
           Vector3.one, new Vector3(1.1f, 1.1f, 1.1f), 1.5f,
           value => bossTitle.localScale = value,
           EasyInOut.EaseOut));

        //BossTitleBar出現動畫
        bossTitleBar.gameObject.SetActive(true);
        RectTransform bossTitleBarRect = bossTitleBar.GetComponent<RectTransform>();

        StartCoroutine(easyInOut.ChangeValue(
            400f, 850f, 1.5f,
            value => bossTitleBarRect.sizeDelta = new Vector2(value, bossTitleBarRect.sizeDelta.y),
            EasyInOut.EaseOut));

        //BossSubTitle出現
        bossSubTitle.gameObject.SetActive(true);

        //BossPanel出現動畫
        bossPanel.gameObject.SetActive(true);
        StartCoroutine(easyInOut.ChangeValue(
            new Vector3(0f,1f,1f), Vector3.one, 1.5f,
            value => bossPanel.localScale = value,
            EasyInOut.EaseInOut));

        float timer = 0f;

        while (timer < showTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        bossPanel.localScale = Vector3.one;

        yield return new WaitForSeconds(0.2f);
        StartCoroutine(MoveBossTitle(0.75f));
    }

    private IEnumerator MoveBossTitle(float showTimer)
    {
        float timer = 0f;
        Image image = bossPanel.GetComponent<Image>();
        Color color = image.color;

        while (timer < showTimer)
        {
            timer += Time.deltaTime;

            color.a = Mathf.Lerp(1, 0, timer / showTimer);
            image.color = color;
            bossTitle.GetComponent<TextMeshProUGUI>().color = color;
            bossSubTitle.GetComponent<TextMeshProUGUI>().color = color;
            bossTitleBar.GetComponent<Image>().color = color;

            yield return null;
        }
        StartCoroutine(ShowBossHP(0.4f));
    }

    private IEnumerator ShowBossHP(float showTimer)
    {
        bossPanel.gameObject.SetActive(false);
        bossTitle.gameObject.SetActive(false);
        bossTitleBar.gameObject.SetActive(false);
        bossSubTitle.gameObject.SetActive(false);

        EasyInOut easyInOut = FindObjectOfType<EasyInOut>();

        //BossHP外框出現動畫
        bossHP.gameObject.SetActive(true);
        var glow = bossHP.transform.Find("Background/Glow").gameObject;
        StartCoroutine(easyInOut.ChangeValue(
            new Vector4(255 / 255f, 255 / 255f, 255 / 255f, 150 / 255f),
            new Vector4(255 / 255f, 74 / 255f, 62 / 255f, 150 / 255f),
            0.25f,
           value => glow.GetComponent<Image>().color = value,
           EasyInOut.EaseOut));

        var background = bossHP.transform.Find("Background").gameObject;
        StartCoroutine(easyInOut.ChangeValue(
            new Vector4(255 / 255f, 255 / 255f, 255 / 255f, 15 / 255f),
            new Vector4(255 / 255f, 74 / 255f, 62 / 255f, 15 / 255f),
            0.25f,
           value => background.GetComponent<Image>().color = value,
           EasyInOut.EaseOut));

        //BossHP漸滿動畫
        var bossCurrentHp = bossHP.transform.Find("HP").gameObject;
        var bossCurrentHpFill = bossHP.transform.Find("HP/Fill").gameObject;

        StartCoroutine(easyInOut.ChangeValue(0f, 1f, 0.75f,
           value => bossCurrentHp.GetComponent<Image>().fillAmount = value,
           EasyInOut.EaseIn));
        yield return new WaitForSeconds(0.75f);

        //0.75秒BossHP滿格後，閃一下白光並演示 BossName出現動畫
        //同時SetActive(true)costHP
        costHP.gameObject.SetActive(true);
        StartCoroutine(easyInOut.ChangeValue(
            new Vector4(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f),
            new Vector4(255 / 255f, 74 / 255f, 62 / 255f, 255 / 255f),
            0.5f,
           value => bossCurrentHpFill.GetComponent<Image>().color = value,
           EasyInOut.EaseOut));

        //BossName出現動畫
        bossName.gameObject.SetActive(true);
        StartCoroutine(easyInOut.ChangeValue(4f, 8f, 0.5f,
           value => bossName.GetComponent<TextMeshProUGUI>().characterSpacing = value,
           EasyInOut.EaseOut));
        StartCoroutine(easyInOut.ChangeValue(
            new Vector3(1.1f, 1.1f, 1.1f), Vector3.one, 0.5f,
           value => bossName.localScale = value,
           EasyInOut.EaseOut));

        //白色閃光
        flash.gameObject.SetActive(true);
        StartCoroutine(easyInOut.ChangeValue(
            new Vector4(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f),
            new Vector4(255 / 255f, 255 / 255f, 255 / 255f, 0f),
            1f,
           value => flash.GetComponent<Image>().color = value,
           EasyInOut.EaseOut));

        float timer = 0f;

        while (timer < showTimer)
        {
            timer += Time.deltaTime;

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

        if (hpCoroutine != null)
        {
            StopCoroutine(hpCoroutine);
        }
        hpCoroutine = StartCoroutine(HPFade(0.3f));
    }
    private IEnumerator HPFade(float fadeTimer)
    {
        float timer = 0;
        Transform fillCost = costHP.transform.Find("FillCost");

        Color color = fillCost.GetComponent<Image>().color;

        while (timer < fadeTimer)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(255f / 255f, 0f, timer / fadeTimer);

            fillCost.GetComponent<Image>().color = color;

            yield return null;
        }
        costHP.fillAmount = currentHP.fillAmount;
    }
}
