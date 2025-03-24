using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RebirthUI : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public UnityEvent openAction;
    public UnityEvent closeAction;

    public Inventory myBag;

    public event Action UseReviveItem;
    public Image blackScreen;
    private EasyInOut easyInOut;
    private void Start()
    {
        blackScreen = GameObject.Find("blackScreen").GetComponent<Image>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.HaveReviveItemDead += ShowRebirthUI;
        }
    }
    private void ShowRebirthUI()
    {
        StartCoroutine(RebirthUIAnimation());
    }

    private IEnumerator RebirthUIAnimation()
    {
        yield return new WaitForSeconds(1.8f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameObject.GetComponent<RebirthUI>().openAction.Invoke();
    }

    public void RevivePlayer()
    {
        UseReviveItem?.Invoke();

        if (playerHealth != null)
        {
            playerHealth.Rivive();
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.GetComponent<RebirthUI>().closeAction.Invoke();
    }
    public void PressExit()
    {
        StartCoroutine(Exit());
    }

    private IEnumerator Exit()
    {
        easyInOut = FindObjectOfType<EasyInOut>();
        gameObject.GetComponent<RebirthUI>().closeAction.Invoke();
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