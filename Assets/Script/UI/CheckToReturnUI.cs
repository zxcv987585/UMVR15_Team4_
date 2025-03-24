using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CheckToReturnUI : MonoBehaviour
{
    public UnityEvent openAction;
    public UnityEvent closeAction;

    private EasyInOut easyInOut;

    public void ShowCheckToReturnUI()
    {
        StartCoroutine(CheckToReturnUIAnimation());
    }

    private IEnumerator CheckToReturnUIAnimation()
    {
        gameObject.GetComponent<CheckToReturnUI>().openAction.Invoke();
        yield return null;
    }

    public void PressExit()
    {
        StartCoroutine(Exit());
    }

    private IEnumerator Exit()
    {
        gameObject.GetComponent<CheckToReturnUI>().closeAction.Invoke();
        yield return new WaitForSecondsRealtime(3f);
        LoadManager.Load(LoadManager.Scene.TitleScene);
    }
}
