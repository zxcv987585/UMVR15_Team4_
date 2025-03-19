using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharactersUIcontroller : MonoBehaviour
{
    //ñ◊ë∂éã‚xìÆ·`
    public UnityEvent openAction;
    public UnityEvent closeAction;
    //ùSéÊåÄèÓUIâÓñ 
    public DialogueTake dialogue;


    // Start is called before the first frame update
    void Start()
    {
        if (dialogue != null) 
        {
            ShowCharactherUI();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ShowCharactherUI()
    {
        StartCoroutine(CharactherUIAnimation());
    }

    private IEnumerator CharactherUIAnimation()
    {
        yield return new WaitForSeconds(1.8f);
        gameObject.GetComponent<CharactersUIcontroller>().openAction.Invoke();
    }
}
