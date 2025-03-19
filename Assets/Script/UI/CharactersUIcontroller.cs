﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharactersUIcontroller : MonoBehaviour
{
    //儲存視窗動畫
    public UnityEvent openAction;
    //抓取劇情UI介面
    public DialogueTake dialogue;
    //抓取unity醬的動畫
    public Animator animator;


    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(UIAnimation());
        dialogue.Take1Finish += enabledUI;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void enabledUI()
    {
        StartCoroutine(UIAnimation());
    }

    private IEnumerator UIAnimation()
    {
        yield return new WaitForSeconds(2.1f);
        gameObject.GetComponent<CharactersUIcontroller>().openAction.Invoke();
        yield return new WaitForSeconds(1.3f);
        animator.Play("WIN00");
    }
}
