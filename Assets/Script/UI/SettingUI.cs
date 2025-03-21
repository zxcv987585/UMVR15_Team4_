using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    public UnityEvent openAction;
    public UnityEvent closeAction;

    private EasyInOut easyInOut;

    public void ShowSettingUI()
    {
        StartCoroutine(SettingUIAnimation());
    }
    public void CloseSettingUI()
    {
        StartCoroutine(SettingUIEvent());
    }

    private IEnumerator SettingUIAnimation()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameObject.GetComponent<SettingUI>().openAction.Invoke();
        yield return null;
    }

    public IEnumerator SettingUIEvent()
    {
        gameObject.GetComponent<SettingUI>().closeAction.Invoke();
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

        yield return new WaitForSeconds(3f);
        Application.Quit();
    }
}