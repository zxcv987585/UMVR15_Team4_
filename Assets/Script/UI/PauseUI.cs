using Michsky.UI.Shift;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public bool isOpen = false;

    public EasyInOut easyInOut;

    public Image blackScreen;
    private Vector4 blackScreenTargetColor = new Vector4(0f, 0f, 0f, 180 / 255f);

    public RectTransform optionBar;
    private Vector2 optionBarDefaultPos = new Vector2(0f, 170f);

    private PlayerController player;

    public RectTransform[] optionButtons;

    public BagPanelManager[] optionList;
    private Animator closeWindow;

    private void Update()
    {
        if(UIManager.CurrentState == UIState.Rebirth && isOpen || UIManager.CurrentState == UIState.MissionFail && isOpen)
        {
            StartCoroutine(RunPauseUI());
        }
    }

    public IEnumerator RunPauseUI()
    {
        AudioManager.Instance.PlaySound("MenuOpen", Vector3.zero);

        //«ö¤U TAB ¶}±Òªº¤èªk
        if (isOpen == false)
        {
            //¶Â«Ì
            StartCoroutine(easyInOut.ChangeValue(
                Vector4.zero,
                blackScreenTargetColor,
                0.15f,
                value => blackScreen.color = value,
                EasyInOut.EaseOut));
            UIManager.CurrentState = UIState.Menu;
            //button·Æ¤J
            foreach (var Button in optionButtons)
            {
                StartCoroutine(easyInOut.ChangeValue(
                new Vector2(Button.GetComponent<RectTransform>().anchoredPosition.x, -70.3f),
                new Vector2(Button.GetComponent<RectTransform>().anchoredPosition.x, -180f),
                0.3f,
                value => Button.anchoredPosition = value,
                EasyInOut.EaseOut));
                yield return new WaitForSeconds(0.05f);
            }

            isOpen = true;
        }
        //«ö¤U TAB Ãö³¬ªº¤èªk
        else if (isOpen == true)
        {
            //Ãö³¬¶}±Ò¤¤ªº Options
            for (int i = 0; i < optionList.Length; i++)
            {
                if (optionList[i].isOn == true)
                {
                    closeWindow = optionList[i].GetComponent<Animator>();
                    closeWindow.CrossFade("Window Out", 0.1f);
                    optionList[i].isOn = false;
                }
            }
            UIManager.CurrentState = UIState.None;
            //¶Â«Ì
            StartCoroutine(easyInOut.ChangeValue(
                blackScreenTargetColor,
                Vector4.zero,
                0.15f,
                value => blackScreen.color = value,
                EasyInOut.EaseOut));

            //button·Æ¥X
            foreach (var Button in optionButtons)
            {
                StartCoroutine(easyInOut.ChangeValue(
                new Vector2(Button.GetComponent<RectTransform>().anchoredPosition.x, -180f),
                new Vector2(Button.GetComponent<RectTransform>().anchoredPosition.x, -70.3f),
                0.2f,
                value => Button.anchoredPosition = value,
                EasyInOut.EaseIn));
                yield return new WaitForSeconds(0.04f);
            }

            isOpen = false;
        }
    }

    public void OpenOption(int index)
    {
        for (int i = 0; i < optionList.Length; i++)
        {
            //ÀË¬d±ø¥ó¡A¸õ¹L­n¶}±Òªº?Eæ¡A¥uÃö³¬¨ä¥Lªº¥B¤w¶}±Òªº?EE
            if (i != index && optionList[i].isOn)
            {
                Animator closeAnim = optionList[i].GetComponent<Animator>();
                closeAnim.CrossFade("Window Out", 0.1f);
                optionList[i].isOn = false;
            }
        }

        //ÀË¬d?Eæ¬O§_¤w³Q¶}±Ò¡A¦pªG¨S¦³´N¶}±Ò
        if (!optionList[index].isOn)
        {
            optionList[index].isOn = true;
            Animator openAnim = optionList[index].GetComponent<Animator>();
            openAnim.CrossFade("Window In", 0.1f);
        }
    }
}
