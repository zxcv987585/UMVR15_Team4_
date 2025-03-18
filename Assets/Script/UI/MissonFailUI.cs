using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MissonFailUI : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public UnityEvent openAction;
    public UnityEvent closeAction;

    private void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.NoReviveItemDead += ShowMissonFailUI;
        }
    }
    private void ShowMissonFailUI()
    {
        StartCoroutine(MissonFailUIAnimation());
    }

    private IEnumerator MissonFailUIAnimation()
    {
        yield return new WaitForSeconds(1.8f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameObject.GetComponent<MissonFailUI>().openAction.Invoke();
    }
}
