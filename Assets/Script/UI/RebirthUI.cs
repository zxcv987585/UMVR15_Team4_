using Michsky.UI.Shift;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RebirthUI : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public UnityEvent openAction;
    public UnityEvent closeAction;

    private void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnDead += ShowRebirthUI;
        }
    }
    private void ShowRebirthUI()
    {
        StartCoroutine(RebirthUIAnimation());
    }

    //等待一秒後撥放死亡視窗畫面
    private IEnumerator RebirthUIAnimation()
    {
        yield return new WaitForSeconds(1f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameObject.GetComponent<RebirthUI>().openAction.Invoke();
    }

    public void RevivePlayer()
    {
        if (playerHealth != null)
        {
            playerHealth.Rivive();
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.GetComponent<RebirthUI>().closeAction.Invoke();
    }
}