using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExitUI : MonoBehaviour
{
    public UnityEvent openAction;
    public UnityEvent closeAction;
    public Image blackScreen;

    private EasyInOut easyInOut;

    public void ShowExitUI()
    {
        StartCoroutine(ExitUIAnimation());
    }
    public void CloseExitUINameUI()
    {
        StartCoroutine(ExitUIEvent());
    }

    private IEnumerator ExitUIAnimation()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameObject.GetComponent<ExitUI>().openAction.Invoke();
        yield return null;
    }

    public IEnumerator ExitUIEvent()
    {
        gameObject.GetComponent<ExitUI>().closeAction.Invoke();
        yield return null;
    }
    public void ExitCheck()
    {
        StartCoroutine(ExitCheckEvent());
    }

    public IEnumerator ExitCheckEvent()
    {
        easyInOut = FindObjectOfType<EasyInOut>();

        gameObject.GetComponent<ExitUI>().closeAction.Invoke();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //¶Â¹õ
        StartCoroutine(easyInOut.ChangeValue(
           Vector4.zero, new Vector4(0f, 0f, 0f, 1f), 3f,
           value => blackScreen.color = value,
           EasyInOut.EaseIn));

        yield return new WaitForSeconds(3f);
        Application.Quit();
    }
}