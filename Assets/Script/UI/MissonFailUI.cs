using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MissonFailUI : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public UnityEvent openAction;
    public UnityEvent closeAction;
    public Image blackScreen;
    private EasyInOut easyInOut;

    private void Start()
    {
        blackScreen = GameObject.Find("blackScreen").GetComponent<Image>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.NoReviveItemDead += ShowMissonFailUI;
        }
    }
    private void ShowMissonFailUI()
    {
        UIManager.CurrentState = UIState.MissionFail;
        StartCoroutine(MissonFailUIAnimation());
    }

    private IEnumerator MissonFailUIAnimation()
    {
        yield return new WaitForSeconds(1.8f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameObject.GetComponent<MissonFailUI>().openAction.Invoke();
    }
    public void PressExit()
    {
        StartCoroutine(Exit());
    }

    private IEnumerator Exit()
    {
        easyInOut = FindObjectOfType<EasyInOut>();
        gameObject.GetComponent<MissonFailUI>().closeAction.Invoke();
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
