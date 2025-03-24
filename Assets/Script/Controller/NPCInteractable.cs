using System;
using System.Collections;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    public BossSceneDialogue TalkUI;
    public Animator animator;
    public RectTransform isUseableUI;
    public Collider NPCcollider;

    public bool isUseable = false;
    public bool isBeUse = false;
    private int TalkCount = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        TalkUI = GameObject.Find("BossSceneDialogueBox").GetComponent<BossSceneDialogue>();
        TalkCount = 0;
}
    private void Start()
    {
        isUseableUI = GameObject.Find("isUseableUI").GetComponent<RectTransform>();
        TalkUI.OnDialogueFinished += OnDialogueFinishedHandler;
        animator.CrossFade("REFLESH00", 1.5f, 0);
    }

    private void OnDialogueFinishedHandler()
    {
        if (isUseable)
        {
            StartCoroutine(isUseableUIon());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isUseable == false && isBeUse == false)
        {
            isUseable = true;
            StartCoroutine(isUseableUIon());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isUseable == true)
        {
            isUseable = false;
            StartCoroutine(isUseableUIoff());
        }
    }

    public void Interact()
    {
        StartCoroutine(isUseableUIoff());

        if (TalkCount == 0)
        {
            TalkCount++;
            this.PlaySound("Uwa");
            TalkUI.FirstTalkToPlayer();
        }
        else if(TalkCount == 1)
        {
            TalkUI.TalkToPlayer();
        }
    }

    private IEnumerator isUseableUIon()
    {
        EasyInOut easyInOut = FindObjectOfType<EasyInOut>();

        StartCoroutine(easyInOut.ChangeValue(
            Vector3.one, new Vector3(1.1f, 1.1f, 1.1f), 0.3f,
            value => isUseableUI.localScale = value,
            EasyInOut.EaseOut
            ));

        StartCoroutine(easyInOut.ChangeValue(
            0f, 1f, 0.3f,
            value => isUseableUI.GetComponent<CanvasGroup>().alpha = value,
            EasyInOut.EaseOut
            ));
        yield return null;
    }
    private IEnumerator isUseableUIoff()
    {
        EasyInOut easyInOut = FindObjectOfType<EasyInOut>();

        StartCoroutine(easyInOut.ChangeValue(
            new Vector3(1.1f, 1.1f, 1.1f), Vector3.one, 0.3f,
            value => isUseableUI.localScale = value,
            EasyInOut.EaseIn
            ));

        StartCoroutine(easyInOut.ChangeValue(
            1f, 0f, 0.3f,
            value => isUseableUI.GetComponent<CanvasGroup>().alpha = value,
            EasyInOut.EaseIn
            ));
        yield return null;
    }
}
