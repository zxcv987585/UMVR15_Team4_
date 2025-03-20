using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputNameUI : MonoBehaviour
{
    public UnityEvent openAction;
    public UnityEvent closeAction;
    
    public TMP_InputField nameInputField;
    public string playerName;

    public void ShowInputNameUI()
    {
        StartCoroutine(InputNameUIAnimation());
    }
    public void CloseInputNameUI()
    {
        StartCoroutine(CloseInputNameEvent());
    }

    private IEnumerator InputNameUIAnimation()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameObject.GetComponent<InputNameUI>().openAction.Invoke();
        yield return null;
    }

    public IEnumerator CloseInputNameEvent()
    {
        playerName = nameInputField.text;
        PlayerPrefs.SetString("PlayerName", playerName);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.GetComponent<InputNameUI>().closeAction.Invoke();

        yield return new WaitForSeconds(2f);
        LoadManager.Load(LoadManager.Scene.Battle01);
    }
}