using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StopUI : MonoBehaviour
{
    public UnityEvent openAction;
    public UnityEvent closeAction;
    public Image blackScreen;
    private EasyInOut easyInOut;

    private bool isOpen = false;

    public event Action ContinueGame;

    private void Start()
    {
        blackScreen = GameObject.Find("blackScreen").GetComponent<Image>();
    }

    public void PressESCUI()
    {
        if (isOpen == false)
        {
            Time.timeScale = 0f;
            StartCoroutine(StopUIAnimation());
            isOpen = true;

            AudioManager.Instance.PlaySound("MenuOpen", transform.position);
        }
        else if (isOpen == true)
        {
            PressContinue();

            AudioManager.Instance.PlaySound("MenuOpen", transform.position);
        }
    }

    private IEnumerator StopUIAnimation()
    {
        gameObject.GetComponent<StopUI>().openAction.Invoke();
        yield return null;
    }

    public void PressContinue()
    {
        Time.timeScale = 1f;
        StartCoroutine(Continue());
        isOpen = false;
        UIManager.CurrentState = UIState.None;
    }

    private IEnumerator Continue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ContinueGame?.Invoke();
        gameObject.GetComponent<StopUI>().closeAction.Invoke();
        yield return null;
    }

    public void PressExit()
    {
        StartCoroutine(Exit());
    }

    private IEnumerator Exit()
    {
        Time.timeScale = 1f;
        easyInOut = FindObjectOfType<EasyInOut>();
        gameObject.GetComponent<StopUI>().closeAction.Invoke();
        StartCoroutine(easyInOut.ChangeValue(
            Vector4.zero,
            new Vector4(0f, 0f, 0f, 1f),
            3f,
           value => blackScreen.color = value,
           EasyInOut.EaseIn));

        yield return new WaitForSecondsRealtime(3f);
        LoadManager.Load(LoadManager.Scene.TitleScene);
    }
}
