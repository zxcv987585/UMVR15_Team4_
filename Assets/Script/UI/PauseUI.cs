using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    private bool isOpen = false;

    public EasyInOut easyInOut;

    public Image blackScreen;
    private Vector4 blackScreenTargetColor = new Vector4(0f, 0f, 0f, 180 / 255f);

    public RectTransform optionBar;
    private Vector2 optionBarDefaultPos = new Vector2(0f, 170f);

    public RectTransform[] optionButtons;

    public IEnumerator RunPauseUI()
    {
        if (isOpen == false)
        {
            //¶Â«Ì
            StartCoroutine(easyInOut.ChangeValue(
                Vector4.zero,
                blackScreenTargetColor,
                0.15f,
                value => blackScreen.color = value,
                EasyInOut.EaseOut));
            //button·Æ¤J
            foreach (var Button in optionButtons)
            {
                StartCoroutine(easyInOut.ChangeValue(
                new Vector2(Button.GetComponent<RectTransform>().anchoredPosition.x, -120f),
                new Vector2(Button.GetComponent<RectTransform>().anchoredPosition.x, -280f),
                0.3f,
                value => Button.anchoredPosition = value,
                EasyInOut.EaseOut));
                yield return new WaitForSeconds(0.05f);
            }

            isOpen = true;
        }
        else if (isOpen == true)
        {
            //¶Â«Ì
            StartCoroutine(easyInOut.ChangeValue(
                blackScreenTargetColor,
                Vector4.zero,
                0.15f,
                value => blackScreen.color = value,
                EasyInOut.EaseOut));

            //button·Æ¥X
            foreach (var Button in optionButtons)
            {
                StartCoroutine(easyInOut.ChangeValue(
                new Vector2(Button.GetComponent<RectTransform>().anchoredPosition.x, -280f),
                new Vector2(Button.GetComponent<RectTransform>().anchoredPosition.x, -120f),
                0.2f,
                value => Button.anchoredPosition = value,
                EasyInOut.EaseIn));
                yield return new WaitForSeconds(0.04f);
            }

            isOpen = false;
        }
    }
}
