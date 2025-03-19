using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharactersUIcontroller : MonoBehaviour
{
    //×¶âx®á`
    public UnityEvent openAction;
    public UnityEvent closeAction;
    //SæîUIîÊ
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
