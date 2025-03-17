using Michsky.UI.Shift;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public bool isOpen = false;

    public EasyInOut easyInOut;

    public Image blackScreen;
    private Vector4 blackScreenTargetColor = new Vector4(0f, 0f, 0f, 180 / 255f);

    public RectTransform optionBar;
    private Vector2 optionBarDefaultPos = new Vector2(0f, 170f);

    public RectTransform[] optionButtons;

    public BagPanelManager[] optionList;
    private Animator closeWindow;

    public IEnumerator RunPauseUI()
    {
        AudioManager.Instance.PlaySound("MenuOpen", Vector3.zero);

        //按下 TAB 開啟的方法
        if (isOpen == false)
        {
            //黑屏
            StartCoroutine(easyInOut.ChangeValue(
                Vector4.zero,
                blackScreenTargetColor,
                0.15f,
                value => blackScreen.color = value,
                EasyInOut.EaseOut));
            //button滑入
            foreach (var Button in optionButtons)
            {
                StartCoroutine(easyInOut.ChangeValue(
                new Vector2(Button.GetComponent<RectTransform>().anchoredPosition.x, -70.3f),
                new Vector2(Button.GetComponent<RectTransform>().anchoredPosition.x, -180f),
                0.3f,
                value => Button.anchoredPosition = value,
                EasyInOut.EaseOut));
                yield return new WaitForSeconds(0.05f);
            }

            isOpen = true;
        }
        //按下 TAB 關閉的方法
        else if (isOpen == true)
        {
            //關閉開啟中的 Options
            for (int i = 0; i < optionList.Length; i++)
            {
                if (optionList[i].isOn == true)
                {
                    closeWindow = optionList[i].GetComponent<Animator>();
                    closeWindow.CrossFade("Window Out", 0.1f);
                    optionList[i].isOn = false;
                }
            }

            //黑屏
            StartCoroutine(easyInOut.ChangeValue(
                blackScreenTargetColor,
                Vector4.zero,
                0.15f,
                value => blackScreen.color = value,
                EasyInOut.EaseOut));

            //button滑出
            foreach (var Button in optionButtons)
            {
                StartCoroutine(easyInOut.ChangeValue(
                new Vector2(Button.GetComponent<RectTransform>().anchoredPosition.x, -180f),
                new Vector2(Button.GetComponent<RectTransform>().anchoredPosition.x, -70.3f),
                0.2f,
                value => Button.anchoredPosition = value,
                EasyInOut.EaseIn));
                yield return new WaitForSeconds(0.04f);
            }

            isOpen = false;
        }
    }

    public void OpenOption(int index)
    {
        for (int i = 0; i < optionList.Length; i++)
        {
            //檢查條件，跳過要開啟的選單，只關閉其他的且已開啟的選單
            if (i != index && optionList[i].isOn)
            {
                Animator closeAnim = optionList[i].GetComponent<Animator>();
                closeAnim.CrossFade("Window Out", 0.1f);
                optionList[i].isOn = false;
            }
        }

        //檢查選單是否已被開啟，如果沒有就開啟
        if (!optionList[index].isOn)
        {
            optionList[index].isOn = true;
            Animator openAnim = optionList[index].GetComponent<Animator>();
            openAnim.CrossFade("Window In", 0.1f);
        }
    }
}
