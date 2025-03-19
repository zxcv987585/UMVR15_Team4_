using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RebirthUI : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public UnityEvent openAction;
    public UnityEvent closeAction;

    public Inventory myBag;

    public event Action UseReviveItem;

    private void Start()
    {
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
}