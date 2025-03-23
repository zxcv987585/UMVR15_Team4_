using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossUIfin : MonoBehaviour
{
    //-----開場動畫------------------------------------
    [SerializeField] private RectTransform bossPanel;
    [SerializeField] private RectTransform bossTitle;
    [SerializeField] private Image bossTitleBar;
    [SerializeField] private RectTransform flash;
    //-------------------------------------------------

    [SerializeField] private Image blackScreen;
    [SerializeField] private BossSceneDialogue sceneDialogue;

    private void Start()
    {
        blackScreen = GameObject.Find("blackScreen").GetComponent<Image>();
        sceneDialogue = GameObject.Find("BossSceneDialogueBox").GetComponent<BossSceneDialogue>();

        blackScreen.color = Color.clear;

        Transform parent = transform.parent;
        if (parent != null)
        {
            transform.SetParent(parent, true);
            transform.SetAsFirstSibling();
        }
        StartCoroutine(ShowBossPanel(3f));
    }

    private IEnumerator ShowBossPanel(float showTimer)
    {
        EasyInOut easyInOut = FindObjectOfType<EasyInOut>();

        //BossTitle出現動畫
        bossTitle.gameObject.SetActive(true);
        StartCoroutine(easyInOut.ChangeValue(0f, 14f, 2.5f,
           value => bossTitle.GetComponent<TextMeshProUGUI>().characterSpacing = value,
           EasyInOut.EaseOut));

        //BossTitleBar出現動畫
        bossTitleBar.gameObject.SetActive(true);
        RectTransform bossTitleBarRect = bossTitleBar.GetComponent<RectTransform>();

        StartCoroutine(easyInOut.ChangeValue(
            750f, 1020f, 2.5f,
            value => bossTitleBarRect.sizeDelta = new Vector2(value, bossTitleBarRect.sizeDelta.y),
            EasyInOut.EaseOut));

        //BossPanel出現動畫
        bossPanel.gameObject.SetActive(true);
        StartCoroutine(easyInOut.ChangeValue(
            new Vector3(0f, 1f, 1f), Vector3.one, 1.5f,
            value => bossPanel.localScale = value,
            EasyInOut.EaseOut));

        //白色閃光
        flash.gameObject.SetActive(true);
        StartCoroutine(easyInOut.ChangeValue(
            new Vector4(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f),
            new Vector4(255 / 255f, 255 / 255f, 255 / 255f, 0f),
            2f,
           value => flash.GetComponent<Image>().color = value,
           EasyInOut.EaseOut));

        float timer = 0f;
        while (timer < showTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        bossPanel.localScale = Vector3.one;

        //黑幕
        StartCoroutine(easyInOut.ChangeValue(
            Vector4.zero, new Vector4(0f, 0f, 0f, 1f), 2f,
           value => blackScreen.GetComponent<Image>().color = value,
           EasyInOut.EaseOut));

        yield return new WaitForSeconds(2f);

        sceneDialogue.transform.SetAsLastSibling();
        sceneDialogue.LastTalk();
    }
}
