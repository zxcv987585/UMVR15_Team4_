using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StopUI : MonoBehaviour
{
    public UnityEvent openAction;
    public UnityEvent closeAction;
    public Image blackScreen;
    private EasyInOut easyInOut;

    private bool isOpen = false;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        blackScreen = GameObject.Find("blackScreen").GetComponent<Image>();
        GameInput.Instance.OnEscape += PressESCUI;
    }

    //訂閱跳轉場景事件
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TitleScene")
        {
            GameInput.Instance.OnEscape -= PressESCUI;
            OnDestroy();
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void PressESCUI()
    {
        if (isOpen == false)
        {
            Time.timeScale = 0f;
            StartCoroutine(StopUIAnimation());
            isOpen = true;
        }
        else if (isOpen == true)
        {
            PressContinue();
        }
    }

    private IEnumerator StopUIAnimation()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameObject.GetComponent<StopUI>().openAction.Invoke();
        yield return null;
    }

    public void PressContinue()
    {
        Time.timeScale = 1f;
        StartCoroutine(Continue());
        isOpen = false;
    }

    private IEnumerator Continue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
