using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class TitleAnimation : MonoBehaviour
{
    public ParticleSystem particle;

    public Image blackScreen;

    public Image titleEleOpen;
    private RectTransform titleEleOpenRect;

    public Image titleEle;

    public Image titleText;
    private RectTransform titleTextRect;

    public Image titleCircle;
    private RectTransform titleCircleRect;

    public RectTransform flash;
    public Image shadowImage;

    public GameObject ButtonList;
    public RectTransform ButtonListStart;
    public RectTransform ButtonListSettings;
    public RectTransform ButtonListExit;

    public InputNameUI inputNameUI;
    public ExitUI exitUI;
    public SettingUI settingUI;

    private float rotationSpeed = 20f;
    private float targetSpeed = 20f;

    private EasyInOut easyInOut;

    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        particle = GameObject.Find("TitleCanvas/Glow Motes").GetComponent<ParticleSystem>();
        blackScreen = GameObject.Find("TitleCanvas/BlackScreen").GetComponent<Image>();
        titleEleOpen = GameObject.Find("TitleCanvas/Title/Mask").GetComponent<Image>();
        titleEle = GameObject.Find("TitleCanvas/Title/ele").GetComponent<Image>();
        titleCircle = GameObject.Find("TitleCanvas/Title/circle").GetComponent<Image>();
        titleText = GameObject.Find("TitleCanvas/Title/text").GetComponent<Image>();
        flash = GameObject.Find("TitleCanvas/Title/flash").GetComponent<RectTransform>();
        shadowImage = GameObject.Find("TitleCanvas/Title/Shadow").GetComponent<Image>();
        ButtonList = GameObject.Find("TitleCanvas/ButtonList");
        ButtonListStart = GameObject.Find("TitleCanvas/ButtonList/StartButton").GetComponent<RectTransform>();
        ButtonListSettings = GameObject.Find("TitleCanvas/ButtonList/SettingsButton").GetComponent<RectTransform>();
        ButtonListExit = GameObject.Find("TitleCanvas/ButtonList/ExitButton").GetComponent<RectTransform>();
        inputNameUI = FindAnyObjectByType<InputNameUI>();
        exitUI = FindAnyObjectByType<ExitUI>();
        settingUI = FindAnyObjectByType<SettingUI>();

        titleEleOpenRect = titleEleOpen.GetComponent<RectTransform>();
        titleCircleRect = titleCircle.GetComponent<RectTransform>();
        titleTextRect = titleText.GetComponent<RectTransform>();
    }

    private void Start()
    {
        particle.Stop();
        titleEle.gameObject.SetActive(false);
        titleCircle.gameObject.SetActive(false);
        titleText.gameObject.SetActive(false);
        flash.gameObject.SetActive(false);
        shadowImage.gameObject.SetActive(false);
        ButtonList.SetActive(false);

        StartCoroutine(titleOpening());
    }

    // Update is called once per frame
    void Update()
    {
        //中心圓框自轉效果
        rotationSpeed = Mathf.Lerp(rotationSpeed, targetSpeed, Time.deltaTime * 5f);

        // 直接使用 Time.deltaTime 計算旋轉量，確保不會因速度變化而回彈
        titleCircleRect.rotation *= Quaternion.Euler(0, 0, rotationSpeed * Time.deltaTime);
    }

    private IEnumerator titleOpening()
    {
        easyInOut = FindObjectOfType<EasyInOut>();
        StartCoroutine(easyInOut.ChangeValue(
           new Vector4(0f, 0f, 0f, 1f), Vector4.zero, 3f,
           value => blackScreen.color = value,
           EasyInOut.EaseIn));

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(easyInOut.ChangeValue(
            0f, 839f, 1f,
            value => titleEleOpenRect.sizeDelta = new Vector2(value, titleEleOpenRect.sizeDelta.y),
            EasyInOut.EaseIn));

        yield return new WaitForSeconds(1f);
        titleEleOpen.gameObject.SetActive(false);
        //-----
        titleEle.gameObject.SetActive(true);
        titleCircle.gameObject.SetActive(true);
        titleText.gameObject.SetActive(true);
        flash.gameObject.SetActive(true);


        //文字標題
        StartCoroutine(easyInOut.ChangeValue(
           new Vector3(1.1f, 1.1f, 1.1f), Vector3.one, 1f,
           value => titleTextRect.localScale = value,
           EasyInOut.EaseOut));

        //中心圓框
        StartCoroutine(easyInOut.ChangeValue(
            new Vector3(1.2f, 1.2f, 1.2f), Vector3.one, 2f,
            value => titleCircleRect.localScale = value,
            EasyInOut.EaseOut));

        //閃光
        StartCoroutine(easyInOut.ChangeValue(
            new Vector4(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f),
            new Vector4(255 / 255f, 255 / 255f, 255 / 255f, 0f),
            1f,
           value => flash.GetComponent<Image>().color = value,
           EasyInOut.EaseOut));

        AudioManager.Instance.PlaySound("TitleAudio", transform.position);

        //底部陰影
        shadowImage.gameObject.SetActive(true);
        StartCoroutine(easyInOut.ChangeValue(
           Vector4.zero,
           new Vector4(0f, 0f, 0f, 0.4f),
           1f,
          value => shadowImage.color = value,
          EasyInOut.EaseOut));

        //粒子效果
        particle.gameObject.SetActive(true);
        particle.Play();

        //選單飛入
        yield return new WaitForSeconds(1.25f);
        ButtonListStart.anchoredPosition = new Vector2(100f, -30f);
        ButtonListSettings.anchoredPosition = new Vector2(100f, -115f);
        ButtonListExit.anchoredPosition = new Vector2(100f, -200f);

        ButtonList.gameObject.SetActive(true);
        StartCoroutine(easyInOut.ChangeValue(
           0f, 1f, 1f,
          value => ButtonList.GetComponent<CanvasGroup>().alpha = value,
          EasyInOut.EaseOut));

    }

    //Start事件--
    public void OnClickStartButton()
    {
        StartCoroutine(StartOpening());
    }

    private IEnumerator StartOpening()
    {
        //關閉所有按鍵互動
        ButtonListStart.GetComponent<Button>().enabled = false;
        ButtonListSettings.GetComponent<Button>().enabled = false;
        ButtonListExit.GetComponent<Button>().enabled = false;

        //閃光
        StartCoroutine(easyInOut.ChangeValue(
            new Vector4(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f),
            new Vector4(255 / 255f, 255 / 255f, 255 / 255f, 0f),
            1f,
           value => flash.GetComponent<Image>().color = value,
           EasyInOut.EaseOut));

        //中心圓框突然加速旋轉
        float originalSpeed = rotationSpeed; //記錄原始速度
        targetSpeed = 200f; //設定短暫加速速度

        StartCoroutine(easyInOut.ChangeValue(
           Vector3.one, new Vector3(1.05f, 1.05f, 1.05f), 0.5f,
           value => titleCircleRect.localScale = value,
           EasyInOut.EaseOut));

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(easyInOut.ChangeValue(
           new Vector3(1.05f, 1.05f, 1.05f), Vector3.one, 0.5f,
           value => titleCircleRect.localScale = value,
           EasyInOut.EaseInOut));

        yield return new WaitForSeconds(0.3f);
        targetSpeed = originalSpeed;

        //黑幕
        StartCoroutine(easyInOut.ChangeValue(
           Vector4.zero, new Vector4(0f, 0f, 0f, 1f), 1.5f,
           value => blackScreen.color = value,
           EasyInOut.EaseIn));

        yield return new WaitForSeconds(2f);
        inputNameUI.ShowInputNameUI();

    }

    //Settings事件--
    public void OnClickSettingButton()
    {
        StartCoroutine(SettingOpening());
    }

    private IEnumerator SettingOpening()
    {
        settingUI.ShowSettingUI();
        yield return null;
    }

    //Exit事件--
    public void OnClickExitButton()
    {
        StartCoroutine(ExitOpening());
    }

    private IEnumerator ExitOpening()
    {
        exitUI.ShowExitUI();
        yield return null;
    }
}
